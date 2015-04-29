using System;
using System.Threading.Tasks;
using VitalChoice.Domain.Mail;

namespace VitalChoice.Business.Mail
{
    public interface INotificationService
    {
	    Task SendUserActivationAsync(string email, UserActivation activation);
    }
}