﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Profiling.Base;

namespace VC.Admin.Models.Settings
{
    public class ProfileScopeListItemModel
    {
        public long Id { get; set; }

        public string Data { get; set; }

        public IEnumerable<string> AdditionalData { get; set; }

        public string ShortData { get; set; }

        public double TimeElapsed { get; set; }

        public string MethodName { get; set; }

        public string ClassTypeName { get; set; }

        public IEnumerable<ProfileScopeListItemModel> SubScopes { get; set; }

        public ProfileScopeListItemModel(ProfilingScope item)
        {
            if (item != null)
            {
                Id = item.Id;
                Data = item.Data?.ToString() ?? string.Empty;
                ShortData = Data.Length > 50
                    ? Data.Substring(0, 50)
                    : Data;
                ClassTypeName = item.ClassType?.FullName ?? string.Empty;
                TimeElapsed = Math.Round(item.TimeElapsed.TotalMilliseconds, 2);
                MethodName = item.MethodName ?? string.Empty;
                SubScopes = item.SubScopes?.Select(p => new ProfileScopeListItemModel(p)).ToArray() ?? Enumerable.Empty<ProfileScopeListItemModel>();
                AdditionalData = item.AdditionalData?.Select(d => d?.ToString()) ?? Enumerable.Empty<string>();
            }
        }
    }
}