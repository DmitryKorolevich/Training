using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Interfaces.Services
{
    interface IContentService
    {
        Task<ExecutedContentItem> GetContent(Dictionary<string, object> parameters);
    }
}
