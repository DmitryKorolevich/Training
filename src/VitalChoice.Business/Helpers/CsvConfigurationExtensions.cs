using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.Configuration;

namespace VitalChoice.Business.Helpers
{
    public static class CsvConfigurationExtensions
    {
        public static void ConfigureDefault(this CsvConfiguration configuration, Type mapType, bool caseSensitive = false)
        {
            configuration.TrimFields = true;
            configuration.TrimHeaders = true;
            configuration.WillThrowOnMissingField = false;
            configuration.IsHeaderCaseSensitive = false;
            configuration.RegisterClassMap(mapType);
        }

        public static void ConfigureDefault(this CsvConfiguration configuration, CsvClassMap map, bool caseSensitive = false)
        {
            configuration.TrimFields = true;
            configuration.TrimHeaders = true;
            configuration.WillThrowOnMissingField = false;
            configuration.IsHeaderCaseSensitive = false;
            configuration.RegisterClassMap(map);
        }

        public static void ConfigureDefault<T>(this CsvConfiguration configuration, bool caseSensitive = false)
            where T : CsvClassMap
        {
            configuration.TrimFields = true;
            configuration.TrimHeaders = true;
            configuration.WillThrowOnMissingField = false;
            configuration.IsHeaderCaseSensitive = false;
            configuration.RegisterClassMap<T>();
        }
    }
}