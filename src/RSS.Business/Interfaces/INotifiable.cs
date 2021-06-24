using RSS.Business.Notifications;
using System.Collections.Generic;

namespace RSS.Business.Interfaces
{
    public interface INotifiable
    {
        bool HasNotification();
        List<Notification> GetNotifications();
        void Handle(Notification notification);
    }
}
