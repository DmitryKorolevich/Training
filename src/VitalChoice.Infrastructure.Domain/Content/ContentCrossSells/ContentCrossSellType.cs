using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Domain.Content.ContentCrossSells
{
    public enum ContentCrossSellType:byte
    {
		Default = 1,
		AddToCart = 2,
		ViewCart = 3
    }
}
