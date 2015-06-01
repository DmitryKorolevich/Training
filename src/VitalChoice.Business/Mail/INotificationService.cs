using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Mail;

namespace VitalChoice.Business.Mail
{
    public interface INotificationService
    {
	    Task SendUserActivationAsync(string email, UserActivation activation);

	    Task SendPasswordResetAsync(string email, PasswordReset passwordReset);

        Task SendBasicEmailAsync(BasicEmail email);
    }
}