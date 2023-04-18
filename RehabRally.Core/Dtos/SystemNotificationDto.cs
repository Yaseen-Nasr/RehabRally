using RehabRally.Core;
using RehabRally.Core.Helpers;

namespace RehabRally.Core.Dtos
{
    public class SystemNotificationDto
    {
        public string Title{ get; set; } = null!;
        public FcmNotificationType Type { get; set; }
        public string Body { get; set; }  = null!;
    }
}
