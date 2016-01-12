﻿namespace VitalChoice.Ecommerce.Domain.Options
{
    public class AppOptionsBase
    {
        public Connection Connection { get; set; }
        public string LogPath { get; set; }
        public string LogLevel { get; set; }
        public CacheSettings CacheSettings { get; set; }
    }

    public class CacheSettings
    {
        public long MaxProcessHeapsSizeBytes { get; set; }
        public int CacheTimeToLeaveSeconds { get; set; }
        public int CacheScanPeriodSeconds { get; set; }
    }
}