using Microsoft.AspNetCore.Mvc.Rendering;
using RehabRally.Core.Helpers;

namespace RehabRally.Core.ViewModels
{
    public class UserReminderViewModel
    {
        public string UserId { get; set; } = null!;
        public int NotificationType { get; set; }
         public IEnumerable<SelectListItem>? NotificationTypeList { get; set; } 
    }
}
