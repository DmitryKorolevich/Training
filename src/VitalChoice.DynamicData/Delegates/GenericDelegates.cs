using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.DynamicData.Delegates
{
    public delegate object GenericGetDelegate(object obj);
    public delegate void GenericSetDelegate(object obj, object value);
}