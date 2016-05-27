using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Core.DependencyInjection;

namespace VitalChoice.Workflow.Configuration
{
    public class WorkflowDependencyConfig: DefaultDependencyConfig
    {
        protected override void StartCustomServicesRegistration(IServiceCollection services)
        {
            base.StartCustomServicesRegistration(services);
            services.AddSingleton<IServer, DummyServer>();
        }
    }
}