using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RehabRally.Core.Models;
using RehabRally.Core.Dtos;


using RehabRally.Core.Helpers;
using System.Reflection;
using System.Security.Claims;
using RehabRally.Ef.Data;
using RehabRally.Core.Abstractions;

namespace RehabRally.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientTasksController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PatientTasksController(UserManager<ApplicationUser> userManager, IMapper mapper , IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        [HttpGet("getMyTasks")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetAllTasks()
        {
            try
            {
                var userId = User.FindFirstValue("uid");
                var exercises = await _unitOfWork.PatientExercises
                                                        .GetQueryable(x => x.UserId == userId,new string[] { nameof(Exercise) }) 
                                                        .Select(e => new
                                                        {
                                                            e.Id,
                                                            e.Exercise!.Title,
                                                            e.Sets,
                                                            e.Repetions,
                                                            e.IsDone 
                                                        })
                                                        .ToListAsync();

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
        public async Task<IActionResult> GetTaskDetails([FromQuery] int taskId)
        {
            var userId = User.FindFirstValue("uid");
            if (userId is null)
                return BadRequest();

            var taskDetails = await _unitOfWork.PatientExercises
                                    .GetQueryable(x => x.Id == taskId && x.UserId == userId, new string[] { nameof(Exercise) }) 
                                    .Select(x => new TaskDetailsDto
                                    {
                                        TaskId = x.Id,
                                        ExerciseId = x.ExerciseId,
                                        IsDone = x.IsDone,
                                        Repetions = x.Repetions,
                                        Sets = x.Sets,
                                        SetsDoneCount = x.SetsDoneCount,
                                        Description = x.Exercise!.Description
                                    }).FirstOrDefaultAsync();
            if (taskDetails is null)
                return NotFound("there's no task!!");

            var exercise = await _unitOfWork.Exercises.GetByIdAsync(taskDetails!.ExerciseId);
            if (exercise is null)
                return NotFound("some thing went wrong!!");

            taskDetails.ImageUrls = GetImageUrlProperties(exercise);

            return Ok(taskDetails);

        }
        //Go To TaskDetails
        [HttpGet("ModifyTaskSetsCount")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> ModifyTaskSetsCount([FromQuery] int taskId)
        {
            var userId = User.FindFirstValue("uid");
            if (userId is null)
                return BadRequest();

            var patientExercise = await _unitOfWork.PatientExercises 
                                        .GetQueryable(x => x.Id == taskId && x.UserId == userId)
                                        .FirstOrDefaultAsync();
            if (patientExercise is null)
                return NotFound("there's no task!!");

            if (patientExercise.IsDone)
                return Ok("You have already Finished it");
            patientExercise.SetsDoneCount += 1;
            patientExercise.IsDone = patientExercise.SetsDoneCount == patientExercise.Sets;
              _unitOfWork.Complete();

            return Ok("Great jop, Keep The work up!!");
        }
        [HttpGet("getMyPrecautions")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetAllPrecautions()
        {
            try
            {
                var userId = User.FindFirstValue("uid");
                List<string> precautions = await _unitOfWork.PatientConclusions
                                                        .GetQueryable(x => x.UserId == userId)
                                                        .Select(e => e.Conclusion)
                                                        .ToListAsync();

                return Ok(precautions);
            }
            catch (Exception ex)
            {
                return BadRequest($"Some Thing Went Wrong {ex.Message} ");
                throw;
            }

        }
        [HttpGet("getSystemNotifications")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> SystemNotifications()
        {
            try
            {
                var userId = User.FindFirstValue("uid");
                List<SystemNotificationDto> Notifications = await _unitOfWork.SystemNotifications
                                                        .GetQueryable(x => x.UserId == userId)
                                                        .Select(e => new SystemNotificationDto
                                                        {
                                                            Title = "RehabRally",
                                                            Type = e.NotificationType,

                                                        })
                                                        .ToListAsync();
                foreach (var notification in Notifications)
                {
                    notification.Body = notification.Type switch
                    {
                        FcmNotificationType.Reminder => "It’s time to do the exercises",
                        FcmNotificationType.Encourage => "Keep going",
                        FcmNotificationType.Precaution => "New precaution",
                    };
                }
                return Ok(Notifications);
            }
            catch (Exception ex)
            {
                return BadRequest($"Some Thing Went Wrong {ex.Message} ");
                throw;
            }

        }
        private List<string> GetImageUrlProperties(Exercise obj)
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
