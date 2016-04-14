using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Profiling.Base;

namespace VC.Admin.Models.Settings
{
    public class ProfileScopeListItemModel
    {
        public string Data { get; set; }

        public string ShortData { get; set; }

        public double TimeElapsed { get; set; }

        public string MethodName { get; set; }

        public string ClassTypeName { get; set; }

        public ICollection<ProfileScopeListItemModel> SubScopes { get; set; }

	    public ProfileScopeListItemModel(ProfilingScope item)
        {
            if(item!=null)
            {
                Data = item.Data.ToString();
                ShortData = Data.Length > BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE
                    ? Data.Substring(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    : Data;
                ClassTypeName = item.ClassType.FullName;
                TimeElapsed = item.TimeElapsed.TotalMilliseconds;
                MethodName = item.MethodName;
                SubScopes = item.SubScopes?.Select(p => new ProfileScopeListItemModel(p)).ToList() ?? new List<ProfileScopeListItemModel>();
            }
        }
    }
}