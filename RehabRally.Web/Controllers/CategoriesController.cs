using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RehabRally.Core.Abstractions;
using RehabRally.Core.Consts;
using RehabRally.Core.Models;
using RehabRally.Core.ViewModels;
using RehabRally.Ef.Data;
using RehabRally.Web.Filters;
using System.Data;
using System.Security.Claims;

namespace RehabRally.Web.Controllers
{
    [Authorize(Roles = AppRoles.Doctor)]
    public class CategoriesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CategoriesController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            var viewModels = _mapper.Map<IEnumerable<CategoryViewModel>>(categories);

            return View(viewModels);
        }
        [HttpGet]
        [AjaxOnly]
        public IActionResult Create()
        {
            return PartialView("_Form");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();


            var category = _mapper.Map<Category>(model);
            await _unitOfWork.Categories.Add(category);
            _unitOfWork.Complete();
            var viewModel = _mapper.Map<CategoryViewModel>(category);

            return PartialView("_CategoryRow", viewModel);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {

            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category is null)
                return NotFound();
            category.IsDeleted = !category.IsDeleted;
            category.LastUpdatedOn = DateTime.Now;
            category.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            _unitOfWork.Complete();
            return Ok(category.LastUpdatedOn.ToString());
        }
        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category is null)
                return NotFound();

            var viewModel = _mapper.Map<CategoryFormViewModel>(category);

            return PartialView("_Form", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var category = await _unitOfWork.Categories.GetByIdAsync(model.Id);

            if (category is null)
                return NotFound();

            category = _mapper.Map(model, category);
            category.LastUpdatedOn = DateTime.Now;
            //category.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;


            _unitOfWork.Complete();

            var viewModel = _mapper.Map<CategoryViewModel>(category);

            return PartialView("_CategoryRow", viewModel);

        }

        public async Task<IActionResult> AllowItem(CategoryFormViewModel model)
        {
            var category = await _unitOfWork.Categories.Find(c => c.Name == model.Name);
            var isAllowed = category is null || category.Id.Equals(model.Id);
            return Json(isAllowed);
        }
    }
}
