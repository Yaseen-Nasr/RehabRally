using RehabRally.Web.Helpers;

namespace RehabRally.Web.Core.Models
{
    public class SystemNotification
    {
        public virtual int Id { get; set; }
        public virtual string UserId { get; set; } = null!;
        public virtual FcmNotificationType NotificationType { get; set; }
        public virtual DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
