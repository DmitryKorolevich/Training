﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Caching.Data;

namespace VitalChoice.Caching.Expressions
{
    public class WhereExpression<T>
    {
        public Expression<Func<T, bool>> Expression { get; set; }
        public ICollection<Condition> Conditions { get; set; }
        public ICollection<Operation> Operations { get; set; }
    }
}