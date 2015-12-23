using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace VitalChoice.Caching.Data
{
    internal class Condition
    {
        public Expression Left { get; set; }
        public Expression Right { get; set; }
    }
}