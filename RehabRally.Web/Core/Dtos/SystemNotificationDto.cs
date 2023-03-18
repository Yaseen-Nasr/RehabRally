using RehabRally.Web.Helpers;

namespace RehabRally.Web.Core.Dtos
{
    public class SystemNotificationDto
    {
        public string Title{ get; set; } = null!;
        public FcmNotificationType Type{ get; set; }
        public string Body { get; set; } = null!;
    }
}
