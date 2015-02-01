using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace VitalChoice.Validation.Helpers.GlobalFilters
{
    public class LoggingExceptionFilter : ExceptionFilterAttribute
    {
        //private static readonly Logger SLog = LkLoggerFactory.GetDefault();

        public override void OnException(ExceptionContext context) {
            base.OnException(context);
        }

        public override Task OnExceptionAsync(ExceptionContext context) {
            return base.OnExceptionAsync(context);
        }

        //public override void OnException(HttpActionExecutedContext context)
        //{
        //    if (context.Exception != null) {
        //        SLog.Error(context.Exception);
        //    }
        //}
    }
}