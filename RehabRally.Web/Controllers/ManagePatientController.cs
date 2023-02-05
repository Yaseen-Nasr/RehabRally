using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using RehabRally.Web.Core.Consts;
using RehabRally.Web.Core.Models;
using RehabRally.Web.Core.ViewModels;
using RehabRally.Web.Data;
using RehabRally.Web.Filters;
using System.Net;

namespace RehabRally.Web.Controllers
{
    [Authorize(Roles =AppRoles.Doctor)]
    public class ManagePatientController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;  
        private readonly UserManager<ApplicationUser> _userManager;

        public ManagePatientController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IMapper mapper)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
        }
         public IActionResult Create(AssignExerciseFormViewModel viewModel)
         {
            if (!ModelState.IsValid)
                return BadRequest();

            var patientExercise = _mapper.Map<PatientExercise>(viewModel);
            _context.PatientExercises.Add(patientExercise);
            _context.SaveChanges();
            //ToDo:: Return viweModel And Attach it with OverView for patient details
            return Ok();
         }
        [AjaxOnly]
        public IActionResult GetCategoryExercises(int categoryId)
        {
            var exercises= _context.Exercises.Where(a => a.CategoryId== categoryId).OrderBy(a => a.Title).ToList();

            var selectedexercises = _mapper.Map<IEnumerable<SelectListItem>>(exercises);
            return Ok(selectedexercises);
        }
         

    }
}
