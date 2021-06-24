using FluentValidation;
using FluentValidation.Results;
using KissLog;
using RSS.Business.Interfaces;
using RSS.Business.Models;
using RSS.Business.Notifications;

namespace RSS.Business.Services
{
    public abstract class BaseService
    {
        private readonly INotifiable _notifiable;

        protected BaseService(INotifiable notifiable)
        {
            _notifiable = notifiable;
        }

        protected void Notify(ValidationResult validationResult)
        {
            foreach (var error in validationResult.Errors)
            {
                Notify(error.ErrorMessage);
            }
        }

        protected void Notify(string message)
        {
            _notifiable.Handle(new Notification(message));
        }

        protected bool ExecuteValidation<TV,TE>(TV validation, TE entity) where TV : AbstractValidator<TE> where TE : Entity
        {
            var validator = validation.Validate(entity);
            if (validator.IsValid) return true;
            Notify(validator);
            return false;
        }

        //protected void ExecuteLoggingError(string errorMessage, string memberName)
        //{
        //    _logger.Error(errorMessage, memberName);
        //}
    }
}
