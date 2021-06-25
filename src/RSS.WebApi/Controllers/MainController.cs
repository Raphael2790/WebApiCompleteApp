using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RSS.Business.Interfaces;
using RSS.Business.Notifications;
using System.Linq;

namespace RSS.WebApi.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        private readonly INotifiable _notifiable;

        protected MainController(INotifiable notifiable)
        {
            _notifiable = notifiable;
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            if (!modelState.IsValid) NotifyModelInvalidError(modelState);
            return CustomResponse();
        }

        private bool ValidOperation()
        {
            return !_notifiable.HasNotification();
        }

        protected ActionResult CustomResponse(object result = null)
        {
            if (ValidOperation())
            {
                return Ok(new { 
                    success = true,
                    data = result
                });
            }

            return BadRequest(new { 
                success = false,
                errors = _notifiable.GetNotifications().Select(n => n.Message)
            });
        }

        protected void NotifyModelInvalidError(ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(e => e.Errors);
            foreach (var error in errors)
            {
                var errMsg = error.Exception == null ? error.ErrorMessage : error.Exception.Message;
                NotifyError(errMsg);
            }
        }

        protected void NotifyError(string errMsg)
        {
            _notifiable.Handle(new Notification(errMsg));
        }
    }
}
