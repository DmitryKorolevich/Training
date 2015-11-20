using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Templates;
using Templates.Data;
using Templates.Exceptions;
using Templates.Runtime;

namespace VitalChoice.Infrastructure.Cache
{
    public class TtlGlobalCache : ITtlGlobalCache
    {
        private readonly Dictionary<IdPair, ITtlTemplate> _cache = new Dictionary<IdPair, ITtlTemplate>();
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
                return _idTemplate.GetHashCode();
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
                    ttlTemplate.Dispose();
                }
            }
        }

        public Task RemoveFromCache(int idMaster, int idContent)
        {
            RemoveFromCacheNoBroadCast(idMaster, idContent);
            return Task.Delay(0);
        }

        public ITtlTemplate GetOrCreateTemplate(string masterTemplate, string template, DateTime dateUpdated,
            DateTime masterDateUpdated, int idMaster, int idContent)
        {
            ITtlTemplate result;
            var searchValue = new IdPair(idMaster, string.IsNullOrEmpty(template) ? 0 : idContent);
            lock (_lockObject)
            {
                if (_cache.TryGetValue(searchValue, out result))
                {
                    if (result.DateCreated != dateUpdated || result.MasterDateCreated != masterDateUpdated)
                    {
                        if (
                            !result.Recompile(masterTemplate + template,
                                new CompileContext(new TemplateOptions {AllowCSharp = true, ForceRemoveWhitespace = true})).Success)
                        {
                            //Update dates so old template using in runtime next request ok
                            result.MasterDateCreated = masterDateUpdated;
                            result.DateCreated = dateUpdated;
                            throw new TemplateCompileException(result.CompileResult.Errors);
                        }
                        result.MasterDateCreated = masterDateUpdated;
                        result.DateCreated = dateUpdated;
                    }
                    return result;
                }
            }
            result = new TtlTemplate(masterTemplate + template,
                new CompileContext(new TemplateOptions {AllowCSharp = true, ForceRemoveWhitespace = true}));
            if (!result.CompileResult.Success)
                throw new TemplateCompileException(result.CompileResult.Errors);
            result.MasterDateCreated = masterDateUpdated;
            result.DateCreated = dateUpdated;
            lock (_lockObject)
            {
                ITtlTemplate cachedResult;
                if (_cache.TryGetValue(searchValue, out cachedResult))
                {
                    result.Dispose();
                    return cachedResult;
                }
                _cache.Add(searchValue, result);
            }
            return result;
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