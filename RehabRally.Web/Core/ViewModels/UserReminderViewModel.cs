using Microsoft.AspNetCore.Mvc.Rendering;
using RehabRally.Web.Helpers;

namespace RehabRally.Web.Core.ViewModels
{
    public class UserReminderViewModel
    {
        public string UserId { get; set; } = null!;
        public int NotificationType { get; set; }
         public IEnumerable<SelectListItem>? NotificationTypeList { get; set; } 
    }
}
