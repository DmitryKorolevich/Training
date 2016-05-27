using System;

namespace VitalChoice.Caching.Relational.Base
{
    public interface IValue: IEquatable<IValue>
    {
        bool IsValid();
        object GetValue();
    }
}