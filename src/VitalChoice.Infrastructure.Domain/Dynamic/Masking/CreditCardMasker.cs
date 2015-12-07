using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Attributes;

namespace VitalChoice.Infrastructure.Domain.Dynamic.Masking
{
    public class CreditCardMasker : ValueMasker
    {
        public override string MaskValue(string value)
        {
            return MaskArea(value, 0, value.Length - 4);
        }
    }
}
