using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using cloudscribe.Web.Pagination;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace VitalChoice.Core.DependencyInjection
{
    public class StorefrontDependencyConfig:DefaultDependencyConfig
    {
	    protected override void FinishCustomRegistrations(ContainerBuilder builder)
	    {
            builder.RegisterType<PaginationLinkBuilder>().As<IBuildPaginationLinks>();
		}
    }
}
