using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace QRProject.Api.Controllers.Base
{
    public class BaseApiHttpActionInvoker : IHttpActionInvoker
    {
        private readonly ApiControllerActionInvoker _invoker = new ApiControllerActionInvoker();

        public Task<HttpResponseMessage> InvokeActionAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            var baseController = actionContext.ControllerContext.Controller as BaseApiController;
            if (baseController != null)
            {
                baseController.Configure(actionContext.ActionDescriptor.ActionName);
            }
            var task = _invoker.InvokeActionAsync(actionContext, cancellationToken);
            return task;
        }
    }
}