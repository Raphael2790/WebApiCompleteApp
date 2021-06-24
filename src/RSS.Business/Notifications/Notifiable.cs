using RSS.Business.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace RSS.Business.Notifications
{
    public class Notifiable : INotifiable
    {
        private List<Notification> _notifications;

        public Notifiable()
        {
            _notifications = new List<Notification>();
        }

        public List<Notification> GetNotifications()
        {
            return _notifications;
        }

        public void Handle(Notification notification)
        {
            _notifications.Add(notification);
        }

        public bool HasNotification()
        {
            return _notifications.Any();
        }
    }
}
