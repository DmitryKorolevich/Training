using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Templates;
using Templates.Data;
using Templates.Exceptions;
using Templates.Runtime;

namespace VitalChoice.ContentProcessing.Cache
{
    public class TtlGlobalCache : ITtlGlobalCache
    {
        private readonly ILogger _logger;

        public TtlGlobalCache(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TtlGlobalCache>();
        }

        private class CachedTemplate
        {
            public ITtlTemplate Template { get; set; }
            public DateTime MasterDate { get; set; }
            public DateTime TemplateDate { get; set; }
        }

        private readonly Dictionary<IdPair, CachedTemplate> _cache = new Dictionary<IdPair, CachedTemplate>();
        private readonly object _lockObject = new object();

        private struct IdPair : IEquatable<IdPair>
        {
            private readonly int _idMaster;
            private readonly int _idTemplate;

            public IdPair(int idMaster, int idTemplate)
            {
                _idTemplate = idTemplate;
                _idMaster = idMaster;
            }

            public bool Equals(IdPair other)
            {
                return other._idMaster == _idMaster && other._idTemplate == _idTemplate;
            }

            public override int GetHashCode()
            {
                return _idTemplate.GetHashCode() ^ _idMaster.GetHashCode() * 347;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is IdPair))
                    return false;
                return Equals((IdPair) obj);
            }

            public static bool operator ==(IdPair one, IdPair another)
            {
                return one.Equals(another);
            }

            public static bool operator !=(IdPair one, IdPair another)
            {
                return !one.Equals(another);
            }
        }

        private void RemoveFromCacheNoBroadCast(int idMaster, int idContent)
        {
            var searchValue = new IdPair(idMaster, idContent);
            lock (_lockObject)
            {
                if (_cache.ContainsKey(searchValue))
                {
                    var ttlTemplate = _cache[searchValue];
                    _cache.Remove(searchValue);
                    ttlTemplate.Template.Dispose();
                }
            }
        }

        public Task RemoveFromCache(int idMaster, int idContent)
        {
            RemoveFromCacheNoBroadCast(idMaster, idContent);
            return Task.Delay(0);
        }

        public ITtlTemplate GetOrCreateTemplate(TemplateCacheParam cacheParams)
        {
            CachedTemplate cache;
            var searchValue = new IdPair(cacheParams.IdMaster, string.IsNullOrWhiteSpace(cacheParams.Template) ? 0 : cacheParams.IdTemplate);
            lock (_lockObject)
            {
                if (_cache.TryGetValue(searchValue, out cache))
                {
                    if (cache.TemplateDate < cacheParams.TemplateUpdateDate || cache.MasterDate < cacheParams.MasterUpdateDate)
                    {
                        if (
                            !cache.Template.Recompile(cacheParams.WholeTemplate,
                                new CompileContext(new TemplateOptions
                                {
                                    AllowCSharp = true,
                                    ForceRemoveWhitespace = true,
                                    Data = cacheParams.ViewContext
                                })).Success)
                        {
                            //Update dates so old template using in runtime next request ok
                            cache.MasterDate = cacheParams.MasterUpdateDate;
                            cache.TemplateDate = cacheParams.TemplateUpdateDate;
                            _logger.LogError("Template recompilation error",
                                new TemplateCompileException(cache.Template.CompileResult.ErrorList));
                        }
                        cache.MasterDate = cacheParams.MasterUpdateDate;
                        cache.TemplateDate = cacheParams.TemplateUpdateDate;
                    }
                    return cache.Template;
                }
            }
            cache = new CachedTemplate
            {
                Template = new TtlTemplate(cacheParams.WholeTemplate,
                    new CompileContext(new TemplateOptions
                    {
                        AllowCSharp = true,
                        ForceRemoveWhitespace = true,
                        Data = cacheParams.ViewContext
                    })),
                MasterDate = cacheParams.MasterUpdateDate,
                TemplateDate = cacheParams.TemplateUpdateDate
            };
            if (!cache.Template.CompileResult.Success)
                throw new TemplateCompileException(cache.Template.CompileResult.ErrorList);
            cache.MasterDate = cacheParams.MasterUpdateDate;
            cache.TemplateDate = cacheParams.TemplateUpdateDate;
            lock (_lockObject)
            {
                CachedTemplate cachedResult;
                if (_cache.TryGetValue(searchValue, out cachedResult))
                {
                    cache.Template.Dispose();
                    return cachedResult.Template;
                }
                _cache.Add(searchValue, cache);
            }
            return cache.Template;
        }

        #region IDisposable

        private volatile bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }
                _disposed = true;
                lock (_lockObject)
                {
                    _cache.Clear();
                }
            }
        }

        ~TtlGlobalCache()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}