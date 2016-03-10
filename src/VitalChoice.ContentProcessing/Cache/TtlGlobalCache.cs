using System;
using System.Collections.Concurrent;
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

        private readonly ConcurrentDictionary<IdPair, CachedTemplate> _cache = new ConcurrentDictionary<IdPair, CachedTemplate>();
        private readonly ConcurrentDictionary<string, ITtlTemplate> _compiledCache = new ConcurrentDictionary<string, ITtlTemplate>();

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
                return _idTemplate.GetHashCode() ^ _idMaster.GetHashCode()*347;
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
            CachedTemplate ttlTemplate;
            if (_cache.TryRemove(searchValue, out ttlTemplate))
            {
                //ttlTemplate.Template.Dispose();
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
            return _cache.AddOrUpdate(searchValue, _ =>
            {
                cache = new CachedTemplate
                {
                    Template = GetTemplate(cacheParams),
                    MasterDate = cacheParams.MasterUpdateDate,
                    TemplateDate = cacheParams.TemplateUpdateDate
                };
                return cache;
            }, (_, cached) =>
            {
                if (cached.TemplateDate < cacheParams.TemplateUpdateDate || cached.MasterDate < cacheParams.MasterUpdateDate)
                {
                    cached.Template = GetTemplate(cacheParams);
                    cached.MasterDate = cacheParams.MasterUpdateDate;
                    cached.TemplateDate = cacheParams.TemplateUpdateDate;
                }
                return cached;
            }).Template;
        }

        private ITtlTemplate GetTemplate(TemplateCacheParam cacheParams)
        {
            return _compiledCache.GetOrAdd(cacheParams.WholeTemplate, whole => CompileNew(cacheParams, whole));
        }

        private static ITtlTemplate CompileNew(TemplateCacheParam cacheParams, string whole)
        {
            var template = new TtlTemplate(whole,
                new CompileContext(new TemplateOptions
                {
                    AllowCSharp = true,
                    ForceRemoveWhitespace = true,
                    Data = cacheParams.ViewContext
                }));
            if (!template.CompileResult.Success)
                throw new TemplateCompileException(template.CompileResult.ErrorList);
            return template;
        }

        #region IDisposable

        private volatile bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _cache.Clear();
                }
                _disposed = true;
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