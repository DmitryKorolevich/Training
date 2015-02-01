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
    public class BaseApiControllerActivator : IHttpControllerActivator
    {
        private readonly DefaultHttpControllerActivator _activator = new DefaultHttpControllerActivator();

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var controller = _activator.Create(request, controllerDescriptor, controllerType);
            var baseController = controller as BaseApiController;
            //if (baseController != null)
            //{
            //    baseController.Configure();
            //}
            return controller;
        }
    }
}