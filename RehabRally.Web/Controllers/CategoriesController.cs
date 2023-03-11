using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RehabRally.Web.Core.Consts;
using RehabRally.Web.Core.Models;
using RehabRally.Web.Core.ViewModels;
using RehabRally.Web.Data;
using RehabRally.Web.Filters;
using System.Data;
using System.Security.Claims;

namespace RehabRally.Web.Controllers
{
    [Authorize(Roles = AppRoles.Doctor)]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public CategoriesController(ApplicationDbContext context, IMapper mapper)
        {

            _context = context;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories =await _context.Categories.AsNoTracking().ToListAsync();
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
           await _context.AddAsync(category);
            await _context.SaveChangesAsync();

            var viewModel = _mapper.Map<CategoryViewModel>(category);

            return PartialView("_CategoryRow", viewModel);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {

            var category = await _context.Categories.FindAsync(id);

            if (category is null)
                return NotFound();
            // if true make it false and same f => t
            category.IsDeleted = !category.IsDeleted;
            category.LastUpdatedOn = DateTime.Now;
            category.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            //tracking..
           await _context.SaveChangesAsync();
            return Ok(category.LastUpdatedOn.ToString());
        }
        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> Edit(int id)
        {
            var category =await _context.Categories.FindAsync(id);
            
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

            var category =await _context.Categories.FindAsync(model.Id);

            if (category is null)
                return NotFound();

            category = _mapper.Map(model, category);
            category.LastUpdatedOn = DateTime.Now;
            //category.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;


           await _context.SaveChangesAsync();

            var viewModel = _mapper.Map<CategoryViewModel>(category);

            return PartialView("_CategoryRow", viewModel);

        }

        public async Task<IActionResult> AllowItem(CategoryFormViewModel model)
        {
            var category =await _context.Categories.SingleOrDefaultAsync(c => c.Name == model.Name);
            var isAllowed = category is null || category.Id.Equals(model.Id);
            return Json(isAllowed);
        }
    }
}
 