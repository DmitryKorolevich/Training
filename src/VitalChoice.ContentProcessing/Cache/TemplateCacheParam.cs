using Microsoft.AspNet.Mvc;
using System;

namespace VitalChoice.ContentProcessing.Cache
{
    public struct TemplateCacheParam
    {
        public string Master { get; set; }
        public string Template { get; set; }
        public string WholeTemplate => Master + Template;
        public DateTime TemplateUpdateDate { get; set; }
        public DateTime MasterUpdateDate { get; set; }
        public int IdMaster { get; set; }
        public int IdTemplate { get; set; }
        public ActionContext ActionContext { get; set; }
    }
}