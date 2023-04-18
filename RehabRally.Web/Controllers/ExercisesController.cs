using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RehabRally.Core.Consts;
using RehabRally.Core.Models;
 
using System.Data;
using System.Linq.Dynamic.Core;
using RehabRally.Core.ViewModels;
using RehabRally.Ef.Data;
using RehabRally.Core.Abstractions;

namespace RehabRally.Web.Controllers
{
    [Authorize(Roles = AppRoles.Doctor)]
    public class ExercisesController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
         private readonly IImageService _IImageService;

        public ExercisesController(  IMapper mapper,
            IWebHostEnvironment webHostEnvironment,
              IImageService iImageService, IUnitOfWork unitOfWork)

        {
             _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _IImageService = iImageService;
            _unitOfWork = unitOfWork;
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


            IQueryable<Exercise> exercises = _unitOfWork.Exercises.GetQueryable(new string[] { nameof(Category) });

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


            await _unitOfWork.Exercises.Add(exercise);
            _unitOfWork.Complete();

            return RedirectToAction(nameof(Details), new { id = exercise.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var Exercise =  await _unitOfWork.Exercises.Find(b => b.Id == id);

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

            var exercise = await _unitOfWork.Exercises.Find(b => b.Id == model.Id);
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
            }
             else if (!string.IsNullOrEmpty(exercise.ImageUrl))
            {
                model.ImageUrl = exercise.ImageUrl;
                model.ImageThumbnailUrl = exercise.ImageThumbnailUrl;
            }

            exercise = _mapper.Map(model, exercise);
            exercise.LastUpdatedOn = DateTime.Now;
            _unitOfWork.Complete();

            return RedirectToAction(nameof(Details), new { id = exercise.Id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {

            var exercise = await _unitOfWork.Exercises.GetByIdAsync(id);

            if (exercise is null)
                return NotFound();
            exercise.IsDeleted = !exercise.IsDeleted;
            exercise.LastUpdatedOn = DateTime.Now;
            _unitOfWork.Complete();
            return Ok();
        }
        public async Task<IActionResult> Details(int id)
        {
            var exercise = await _unitOfWork.Exercises
                .Find(b => b.Id == id, new string[] { nameof(Category) });
            if (exercise is null)
                return NotFound();
            var viewModel = _mapper.Map<ExerciseViewModel>(exercise);
            return View(viewModel);
        }



        public async Task<IActionResult> AllowItem(ExerciseFormViewModel model)
        {
            var exerise = await _unitOfWork.Exercises.Find(b => b.Title == model.Title && b.CategoryId == model.CategoryId);
            var isAllowed = exerise is null || exerise.Id.Equals(model.Id);
            return Json(isAllowed);
        }
        private async Task<ExerciseFormViewModel> PopulateViewModel(ExerciseFormViewModel? model = null)
        {
            ExerciseFormViewModel viewModel = model is null ? new ExerciseFormViewModel() : model;
            var categories = await _unitOfWork.Categories.FindAllAsync(a => !a.IsDeleted,null,null,orderBy: a => a.Name);
            viewModel.Categories = _mapper.Map<IEnumerable<SelectListItem>>(categories);
            return viewModel;
        }
         
    }
}

