using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Transfer.TemplateModels.ProductPage
{
    public class TtlProductPageTabContent
    {
	    public string TitleOverride { get; set; }

	    public string Content { get; set; }

	    public bool Hidden { get; set; }
    }
}
