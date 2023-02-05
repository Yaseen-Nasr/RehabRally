using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RehabRally.Web.Core.Dtos;
using RehabRally.Web.Core.Models;
using RehabRally.Web.Data;
using System.Reflection;
using System.Security.Claims;

namespace RehabRally.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientTasksController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public PatientTasksController(UserManager<ApplicationUser> userManager, IMapper mapper, ApplicationDbContext context)
        {
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
        }
        [HttpGet("getMyTasks")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetAllExercises()
        {
            try
            {
                var userId = User.FindFirstValue("uid");
                var exercises = await _context.PatientExercises.Where(x => x.UserId == userId && !x.IsDone).ToListAsync();

                return Ok(exercises);
            }
            catch (Exception ex)
            {
                return BadRequest($"Some Thing Went Wrong {ex.Message} ");
                throw;
            }

        }
        //Go To TaskDetails
        [HttpGet("GetTaskDetails")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetTaskDetails(int taskId)
        {
            var userId = User.FindFirstValue("uid");
            if (userId is null)
                return BadRequest();

            var taskDetails = await _context.PatientExercises
                                    .Where(x => x.Id == taskId && x.UserId == userId)
                                        .Select(x => new TaskDetailsDto
                                        {
                                            TaskId = x.Id,
                                            ExerciseId = x.ExerciseId,
                                            IsDone = x.IsDone,
                                            Repetions = x.Repetions,
                                            Sets = x.Sets,
                                            SetsDoneCount = x.SetsDoneCount,
                                        }).FirstOrDefaultAsync();
            if (taskDetails is null)
                return NotFound("there's no task!!");

            var exercise = await _context.Exercises.FindAsync(taskDetails!.ExerciseId);
            if (exercise is null)
                return NotFound("some thing went wrong!!");

            taskDetails.ImageUrls = GetStringProperties(exercise);

            return Ok(taskDetails);

        }

        private List<string> GetStringProperties(Exercise obj)
        {
            List<string> propertyValues = new List<string>();
            PropertyInfo[] properties = obj.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType == typeof(string) && property.Name.StartsWith("Image"))
                {
                    string value = (string)property.GetValue(obj)!;
                    if (!string.IsNullOrEmpty(value))
                    {
                        propertyValues.Add(value);
                    }
                }
            }

            return propertyValues;
        }

    }
}
