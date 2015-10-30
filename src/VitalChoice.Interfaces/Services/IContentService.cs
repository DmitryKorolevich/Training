using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Interfaces.Services
{
    interface IContentService
    {
        Task<ContentViewModel> GetContent(Dictionary<string, object> parameters);
    }
}
