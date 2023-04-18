using Microsoft.AspNetCore.Identity; 
using Microsoft.AspNetCore.Mvc.Rendering;
using RehabRally.Core.Models; 
using System.Data;
using RehabRally.Core.ViewModels; 
using FirebaseAdmin.Messaging; 
 using RehabRally.Ef.Data;
using RehabRally.Core.Abstractions;

namespace RehabRally.Web.Controllers
{
    [Authorize(Roles = AppRoles.Doctor)]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager; 
        private readonly IUnitOfWork _unitOfWork;  
        private readonly IMapper _mapper;
        private readonly FirebaseMessaging _firebaseMessaging;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper , 
            FirebaseMessaging firebaseMessaging, 
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
             _firebaseMessaging = firebaseMessaging; 
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var viewModel = _mapper.Map<IEnumerable<UserViewModel>>(users.Where(u => u.Email != "mohamed@rehabrally.com"));
            return View(viewModel);
        }
        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> Create()
        {
            UserFormViewModel viewModel = new();
            return PartialView("_Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            ApplicationUser user = new()
            {
                FullName = model.FullName,
                UserName = model.UserName,
                Email = model.Email,
                MobileNumber = model.MobileNumber,
                Age = model.Age,
                CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            };
            var results = await _userManager.CreateAsync(user, model.Password);
            if (results.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, AppRoles.Patient);
                return PartialView("_UserRow", _mapper.Map<UserViewModel>(user));
            };

            return BadRequest(string.Join(", ", results.Errors.Select(e => e.Description)));
        }
        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return NotFound();
            var viewModel = _mapper.Map<UserFormViewModel>(user);
            return PartialView("_Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user is null)
                return NotFound();
            user = _mapper.Map(model, user);
            user.LastUpdatedOn = DateTime.Now;
            user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                var viewModel = _mapper.Map<UserViewModel>(user);
                return PartialView("_UserRow", viewModel);
            }

            return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> Reminder(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return NotFound();

            var viewModel = new UserReminderViewModel()
            {
                UserId = user.Id,
                NotificationTypeList = Enum.GetValues(typeof(FcmNotificationType)).Cast<FcmNotificationType>().Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList()
            };


            return PartialView("_Reminder", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reminder(UserReminderViewModel viewModel)
        {


            var fireBaseTokens = await _unitOfWork.RegisterdMashines.GetQueryable(r => r.UserId == viewModel.UserId) 
                                                                    .Select(r => r.FirebaseToken).ToListAsync();
            if (!fireBaseTokens.Any())
                return NotFound("The Current User Dose Not login ");
            foreach (var token in fireBaseTokens)
            {
                var message = new Message()
                {
                    Notification = new Notification()
                    {
                        Title = "RehabRally",
                        Body = ((FcmNotificationType)viewModel.NotificationType).ToString(),
                    },

                    Token = token,

                    Data = new Dictionary<string, string>
                         {
                             { "Title", "RehabRally" },
                             { "Body", ((FcmNotificationType)viewModel.NotificationType).ToString()},
                             { "Type", viewModel.NotificationType.ToString() },
                          }
                };
                try
                {
                    var result = await _firebaseMessaging.SendAsync(message);
                }
                catch (Exception)
                {

                    throw new Exception("This usert Is not Register");
                }

            }
            await _unitOfWork.SystemNotifications.Add(new SystemNotification
            {
                UserId = viewModel.UserId,
                NotificationType = (FcmNotificationType)viewModel.NotificationType,

            });
              _unitOfWork.Complete();
            return Ok();
        }



        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return NotFound();

            var viewModel = new ResetPasswordFormViewModel()
            {
                Id = id,
            };

            return PartialView("_ResetPassword", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user is null)
                return NotFound();

            var currentPaswwordHash = user.PasswordHash;
            await _userManager.RemovePasswordAsync(user);
            var result = await _userManager.AddPasswordAsync(user, model.Password);
            if (result.Succeeded)
            {
                user.LastUpdatedOn = DateTime.Now;
                user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                await _userManager.UpdateAsync(user);
                var viewModel = _mapper.Map<UserViewModel>(user);
                return PartialView("_UserRow", viewModel);
            }
            user.PasswordHash = currentPaswwordHash;
            await _userManager.UpdateAsync(user);
            return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));

        }
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return BadRequest();
            var viewModel = _mapper.Map<UserViewModel>(user);
            viewModel.AssignExercise = new AssignExerciseFormViewModel();
            var categories =await _unitOfWork.Categories.GetQueryable(a => !a.IsDeleted).OrderBy(a => a.Name).ToListAsync();

            viewModel.AssignExercise.UserId = user.Id;
            viewModel.AssignExercise.Categories = _mapper.Map<IEnumerable<SelectListItem>>(categories);
            viewModel.PatientExercises = await _unitOfWork.PatientExercises.GetQueryable(a => a.UserId == user.Id, new string[] { nameof(Exercise) })
                                                       .Select(e => new PatientExerciseViewModel()
                                                       {
                                                           Exercise = e.Exercise!.Title,
                                                           IsDone = e.IsDone,
                                                           Repetions = e.Repetions,
                                                           Sets = e.Sets,
                                                           CreatedOn = e.CreatedOn,
                                                           SetsDoneCount = e.SetsDoneCount,
                                                       }).ToListAsync();
            viewModel.Precautions = await _unitOfWork.PatientConclusions.GetQueryable(p => p.UserId == user.Id).Select(p => p.Conclusion).ToListAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddConclusion(string userId, string conclusion)
        {

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return NotFound();
            await _unitOfWork.PatientConclusions.Add(new PatientConclusion { UserId = userId, Conclusion = conclusion });
              _unitOfWork.Complete();
            return RedirectToAction("Details", new { id = user.Id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {

            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return NotFound();
            // if true make it false and same f => t
            user.IsDeleted = !user.IsDeleted;
            user.LastUpdatedOn = DateTime.Now;
            user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            //tracking..
            await _userManager.UpdateAsync(user);
            return Ok(user.LastUpdatedOn.ToString());
        }
        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> AssignExerciseToPatient(string userId)
        {
            var viewModel = new AssignExerciseFormViewModel()
            {
                UserId = userId,
            };
            viewModel.Categories = _mapper.Map<IEnumerable<SelectListItem>>(await _unitOfWork.Categories.GetQueryable().ToListAsync());

            return PartialView("_AssignTask", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignExerciseToPatient(AssignExerciseFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var patientExercise = _mapper.Map<PatientExercise>(viewModel);
            await _unitOfWork.PatientExercises.Add(patientExercise);
            _unitOfWork.Complete();
             var patientExerciseVM = new PatientExerciseViewModel()
            {
                CreatedOn = patientExercise.CreatedOn,
                IsDone = false,
                Repetions = patientExercise.Repetions,
                Sets = patientExercise.Sets,
                SetsDoneCount = patientExercise.SetsDoneCount
            };
            patientExerciseVM.Exercise = await _unitOfWork.Exercises.GetQueryable(s => s.Id == patientExercise.ExerciseId)
                                                                .Select(s => s.Title).SingleOrDefaultAsync() ?? "";
            return PartialView("_PatientExerciseRow", patientExerciseVM);
        }


        public async Task<IActionResult> GetCategoryExercises(int categoryId)
        {
            var exercises = await _unitOfWork.Exercises.GetQueryable(a => a.CategoryId == categoryId).OrderBy(a => a.Title).ToListAsync();

            var selectedexercises = _mapper.Map<IEnumerable<SelectListItem>>(exercises);
            return Ok(selectedexercises);
        }

        public async Task<IActionResult> AllowUserName(UserFormViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            var isAllowed = user is null || user.Id.Equals(model.Id);
            return Json(isAllowed);
        }
        public async Task<IActionResult> AllowEmail(UserFormViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var isAllowed = user is null || user.Id.Equals(model.Id);
            return Json(isAllowed);
        }
    }
}
