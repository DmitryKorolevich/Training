using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Domain.Transfer
{
    public class KeyExchange
    {
        public byte[] Key { get; set; }

        public byte[] IV { get; set; }
    }
}
