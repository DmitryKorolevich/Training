using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using VitalChoice.Infrastructure.Domain.Avatax;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VitalChoice.Core.Services
{
    public interface IPageResultService
    {
        RedirectResult GetResult(PageResult type);
    }

    public class PageResultService : IPageResultService
    {
        public RedirectResult GetResult(PageResult type)
        {
            if (type == PageResult.NotFound)
                return new RedirectResult("/content/" + ContentConstants.NOT_FOUND_PAGE_URL);
            if (type == PageResult.Forbidden)
                return new RedirectResult("/content/" + ContentConstants.ACESS_DENIED_PAGE_URL);

            throw new Exception("Page type not implemented");
        }
    }

    public enum PageResult
    {
        NotFound,
        Forbidden
    }
}
