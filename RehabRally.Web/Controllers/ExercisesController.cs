using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RehabRally.Web.Core.Consts;
using RehabRally.Web.Core.Models;
using RehabRally.Web.Core.ViewModels;
using RehabRally.Web.Data;
using RehabRally.Web.Filters;
using RehabRally.Web.Services;
using System.Data;
using System.Linq.Dynamic.Core;
using System.Security.Principal;

namespace RehabRally.Web.Controllers
{
    [Authorize(Roles = AppRoles.Doctor)]
    public class ExercisesController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        //private readonly Cloudinary _cloudinary;
        private readonly IImageService _IImageService;


        //IOptions interface to bind data from appsettings to Class service
        public ExercisesController(ApplicationDbContext context, IMapper mapper,
            IWebHostEnvironment webHostEnvironment,
              IImageService iImageService)
        //IOptions<CloudinarySettings> cloudinary

        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            //Account account = new()
            //{
            //    ApiKey = cloudinary.Value.APiKey,
            //    ApiSecret = cloudinary.Value.APiSecret,
            //    Cloud = cloudinary.Value.Cloud
            //};
            //_cloudinary = new Cloudinary(account);
            _IImageService = iImageService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [AjaxOnly]
        [HttpPost]
        public IActionResult GetExercises()
        {
            var skip = int.Parse(Request.Form["start"]);
            var pageSize = int.Parse(Request.Form["length"]);

            var sortColumnIndex = Request.Form["order[0][column]"];
            var sortColumn = Request.Form[$"columns[{sortColumnIndex}][name]"];
            var sortOrderDirection = Request.Form["order[0][dir]"];
            var searchValue = Request.Form["search[value]"];


            IQueryable<Exercise> exercises = _context.Exercises
                .Include(b => b.Category);

            if (!string.IsNullOrEmpty(searchValue))
                exercises = exercises.Where(b => b.Title.Contains(searchValue) || b.Category!.Name.Contains(searchValue));
            exercises = exercises.OrderBy($"{sortColumn} {sortOrderDirection}");
            var data = exercises.Skip(skip).Take(pageSize).ToList();
            var recordsTotal = exercises.Count();
            var mappedData = _mapper.Map<IEnumerable<ExerciseViewModel>>(data);
            var jasonData = new
            {
                recordsFiltered = recordsTotal,
                recordsTotal,
                data = mappedData
            };
            return Ok(jasonData);
        }

        public async Task<IActionResult> Create()
        {
            return View("Form", await PopulateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExerciseFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", await PopulateViewModel(model));

            var exercise = _mapper.Map<Exercise>(model);

            if (model.Image is not null)
            {
                var imageName = $"{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";
                var (isUploaded, errorMessage) = await _IImageService.UploadFiles(model.Image, imageName, UploadedFiles.ExercisesImages, hasThumbnail: true);
                if (!isUploaded)
                {

                    ModelState.AddModelError(nameof(model.Image), errorMessage: errorMessage!);
                    return View("Form", PopulateViewModel(model));
                }

                exercise.ImageUrl = $"{UploadedFiles.ExercisesImages}{imageName}";
                exercise.ImageThumbnailUrl = $"{UploadedFiles.ExercisesImagesThumnail}{imageName}";
            }


            await _context.AddAsync(exercise);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = exercise.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var Exercise = await _context.Exercises.SingleOrDefaultAsync(b => b.Id == id);

            if (Exercise is null)
                return NotFound();

            var model = _mapper.Map<ExerciseFormViewModel>(Exercise);
            var viewModel = await PopulateViewModel(model);

            return View("Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ExerciseFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", PopulateViewModel(model));

            var exercise = await _context.Exercises.SingleOrDefaultAsync(b => b.Id == model.Id);
            //string imagePublicId = null;
            if (exercise is null)
                return NotFound();

            if (model.Image is not null)
            {
                if (!string.IsNullOrEmpty(exercise.ImageUrl))
                {
                    _IImageService.Delete(exercise.ImageUrl, exercise.ImageThumbnailUrl);
                    //await _cloudinary.DeleteResourcesAsync(book.ImagePublicId);
                }

                var imageName = $"{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";
                var (isUploaded, errorMessage) = await _IImageService.UploadFiles(model.Image, imageName, UploadedFiles.ExercisesImages, hasThumbnail: true);
                if (!isUploaded)
                {
                    ModelState.AddModelError(nameof(model.Image), errorMessage: errorMessage!);
                    return View("Form", await PopulateViewModel(model));

                }
                model.ImageUrl = $"{UploadedFiles.ExercisesImages}{imageName}";
                model.ImageThumbnailUrl = $"{UploadedFiles.ExercisesImagesThumnail}{imageName}";
                //call cloudaniry service
                //  using var stream = model.Image.OpenReadStream();
                //  var imageParams = new ImageUploadParams()
                //  {
                //      File = new FileDescription(imageName, stream),
                //      UseFilename = true
                //  };
                //  var result = await _cloudinary.UploadAsync(imageParams);
                //  book.ImageUrl = result.SecureUrl.ToString();
                //imagePublicId = result.PublicId;
            }
            //if user does not edit  image get the old image url
            else if (!string.IsNullOrEmpty(exercise.ImageUrl))
            {
                model.ImageUrl = exercise.ImageUrl;
                model.ImageThumbnailUrl = exercise.ImageThumbnailUrl;
            }

            exercise = _mapper.Map(model, exercise);
            exercise.LastUpdatedOn = DateTime.Now;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = exercise.Id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {

            var exercise = await _context.Exercises.FindAsync(id);

            if (exercise is null)
                return NotFound();
            exercise.IsDeleted = !exercise.IsDeleted;
            exercise.LastUpdatedOn = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok();
        }
        public async Task<IActionResult> Details(int id)
        {
            var exercise = await _context.Exercises
                .Include(a => a.Category)
                .SingleOrDefaultAsync(b => b.Id == id);
            if (exercise is null)
                return NotFound();
            var viewModel = _mapper.Map<ExerciseViewModel>(exercise);
            return View(viewModel);
        }



        public async Task<IActionResult> AllowItem(ExerciseFormViewModel model)
        {
            var exerise =await _context.Exercises.SingleOrDefaultAsync(b => b.Title == model.Title && b.CategoryId == model.CategoryId);
            var isAllowed = exerise is null || exerise.Id.Equals(model.Id);
            return Json(isAllowed);
        }
        private async Task<ExerciseFormViewModel> PopulateViewModel(ExerciseFormViewModel? model = null)
        {
            ExerciseFormViewModel viewModel = model is null ? new ExerciseFormViewModel() : model;
            var categories = await _context.Categories.Where(a => !a.IsDeleted).OrderBy(a => a.Name).ToListAsync();
            viewModel.Categories = _mapper.Map<IEnumerable<SelectListItem>>(categories);
            return viewModel;
        }
        ////Get image Thumbnail Url
        //private string GetThumbnailUrl(string url)
        //{
        //    //c_thumb,w_200,g_face/
        //    //https://res.cloudinary.com/bookifycloudinary/image/upload/cv1667843217/29e33d3b-93ad-4419-bb4f-375810c412d4_zkae7y.jpg
        //    string separator = "image/upload/";
        //    var urlParts = url.Split(separator);
        //    var thumbnailUrl = $"{urlParts[0]}{separator}c_thumb,w_200,g_face/{urlParts[1]}";
        //    return thumbnailUrl;

        //}
    }
}

