using System;
using System.Threading.Tasks;
using Templates;

namespace VitalChoice.ContentProcessing.Cache
{
    public interface ITtlGlobalCache : IDisposable

    {
        Task RemoveFromCache(int idMaster, int idContent);

        ITtlTemplate GetOrCreateTemplate(TemplateCacheParam cacheParams);
    }
}