using AutoMapper;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using RehabRally.Web.Core.Models;
using RehabRally.Web.Filters;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using RehabRally.Web.Core.Consts;
using System.Data;
using RehabRally.Web.Core.ViewModels;
using RehabRally.Web.Data;
using RehabRally.Web.Helpers;
using System;

namespace RehabRally.Web.Controllers
{
    [Authorize(Roles = AppRoles.Doctor)]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper
,
            ApplicationDbContext context
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _context = context;
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
            var categories = _context.Categories.Where(a => !a.IsDeleted).OrderBy(a => a.Name).ToList();

            viewModel.AssignExercise.UserId = user.Id;
            viewModel.AssignExercise.Categories = _mapper.Map<IEnumerable<SelectListItem>>(categories);
            viewModel.PatientExercises = _context.PatientExercises.Include(pa => pa.Exercise).Where(a => a.UserId == user.Id)
                                                       .Select(e => new PatientExerciseViewModel()
                                                       {
                                                           Exercise = e.Exercise!.Title,
                                                           IsDone = e.IsDone,
                                                           Repetions = e.Repetions,
                                                           Sets = e.Sets,
                                                           CreatedOn = e.CreatedOn,
                                                           SetsDoneCount = e.SetsDoneCount,
                                                       }).ToList();
            viewModel.Precautions = _context.PatientConclusions.Where(p => p.UserId == user.Id).Select(p => p.Conclusion).ToList();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddConclusion(string userId, string conclusion)
        {

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return NotFound();
            await _context.AddAsync(new PatientConclusion { UserId = userId, Conclusion = conclusion });
            await _context.SaveChangesAsync();
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
            viewModel.Categories = _mapper.Map<IEnumerable<SelectListItem>>(await _context.Categories.ToListAsync());

            return PartialView("_AssignTask", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignExerciseToPatient(AssignExerciseFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var patientExercise = _mapper.Map<PatientExercise>(viewModel);
            await _context.PatientExercises.AddAsync(patientExercise);
            await _context.SaveChangesAsync();
            //ToDo:: Return viweModel And Attach it with OverView for patient details
            var patientExerciseVM = new PatientExerciseViewModel()
            {
                CreatedOn = patientExercise.CreatedOn,
                IsDone = false,
                Repetions = patientExercise.Repetions,
                Sets = patientExercise.Sets,
                SetsDoneCount = patientExercise.SetsDoneCount
            };
            patientExerciseVM.Exercise = await _context.Exercises.Where(s => s.Id == patientExercise.ExerciseId).Select(s => s.Title).SingleOrDefaultAsync() ?? "";
            return PartialView("_PatientExerciseRow", patientExerciseVM);
        }


        public async Task<IActionResult> GetCategoryExercises(int categoryId)
        {
            var exercises = await _context.Exercises.Where(a => a.CategoryId == categoryId).OrderBy(a => a.Title).ToListAsync();

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
