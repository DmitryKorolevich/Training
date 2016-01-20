using System;

namespace VitalChoice.Ecommerce.Domain.Mail
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IgnoreUserTemplateUseAttribute : Attribute
    {
    }
}