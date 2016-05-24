using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Caching.Relational.Base
{
    internal struct IntValue : IValue
    {
        public readonly int Value;

        public IntValue(int value)
        {
            Value = value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public bool Equals(IValue other)
        {
            if (other is NullValue)
            {
                return false;
            }
            return ((IntValue)other).Value == Value;
        }

        public bool IsValid()
        {
            return Value > 0;
        }

        public object GetValue()
        {
            return Value;
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }

    internal struct StringValue : IValue
    {
        public readonly string Value;

        public StringValue(string value)
        {
            Value = value;
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        public bool Equals(IValue other)
        {
            if (other is NullValue)
            {
                return false;
            }
            return string.Equals(((StringValue)other).Value, Value, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsValid()
        {
            return true;
        }

        public object GetValue()
        {
            return Value;
        }

        public override string ToString()
        {
            return Value;
        }
    }

    internal struct LongValue : IValue
    {
        public readonly long Value;

        public LongValue(long value)
        {
            Value = value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public bool Equals(IValue other)
        {
            if (other is NullValue)
            {
                return false;
            }
            return ((LongValue)other).Value == Value;
        }

        public bool IsValid()
        {
            return Value > 0;
        }

        public object GetValue()
        {
            return Value;
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }

    internal struct GuidValue : IValue
    {
        public readonly Guid Value;

        public GuidValue(Guid value)
        {
            Value = value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public bool Equals(IValue other)
        {
            if (other is NullValue)
            {
                return false;
            }
            return ((GuidValue)other).Value == Value;
        }

        public bool IsValid()
        {
            return Value != Guid.Empty;
        }

        public object GetValue()
        {
            return Value;
        }

        public override string ToString()
        {
            return Value.ToString("D");
        }
    }

    internal struct ShortValue : IValue
    {
        public readonly short Value;

        public ShortValue(short value)
        {
            Value = value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public bool Equals(IValue other)
        {
            if (other is NullValue)
            {
                return false;
            }
            return ((ShortValue)other).Value == Value;
        }

        public bool IsValid()
        {
            return Value > 0;
        }

        public object GetValue()
        {
            return Value;
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }

    internal struct BoolValue : IValue
    {
        public readonly bool Value;

        public BoolValue(bool value)
        {
            Value = value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public bool Equals(IValue other)
        {
            if (other is NullValue)
            {
                return false;
            }
            return ((BoolValue)other).Value == Value;
        }

        public bool IsValid()
        {
            return true;
        }
        
        public object GetValue()
        {
            return Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    internal struct CharValue : IValue
    {
        public readonly char Value;

        public CharValue(char value)
        {
            Value = value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public bool Equals(IValue other)
        {
            if (other is NullValue)
            {
                return false;
            }
            return ((CharValue)other).Value == Value;
        }

        public bool IsValid()
        {
            return Value != '\0';
        }

        public object GetValue()
        {
            return Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    internal struct ByteValue : IValue
    {
        public readonly byte Value;

        public ByteValue(byte value)
        {
            Value = value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public bool Equals(IValue other)
        {
            if (other is NullValue)
            {
                return false;
            }
            return ((ByteValue)other).Value == Value;
        }

        public bool IsValid()
        {
            return Value > 0;
        }

        public object GetValue()
        {
            return Value;
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }

    internal struct DecimalValue : IValue
    {
        public readonly decimal Value;

        public DecimalValue(decimal value)
        {
            Value = value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public bool Equals(IValue other)
        {
            if (other is NullValue)
            {
                return false;
            }
            return ((DecimalValue)other).Value == Value;
        }

        public bool IsValid()
        {
            return Value > 0;
        }

        public object GetValue()
        {
            return Value;
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }

    internal struct DateTimeValue : IValue
    {
        public readonly DateTime Value;

        public DateTimeValue(DateTime value)
        {
            Value = value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public bool Equals(IValue other)
        {
            if (other is NullValue)
            {
                return false;
            }
            return ((DateTimeValue)other).Value == Value;
        }

        public bool IsValid()
        {
            return Value != DateTime.MinValue;
        }

        public object GetValue()
        {
            return Value;
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }

    internal struct NullValue : IValue
    {
        public override int GetHashCode()
        {
            return 0;
        }

        public bool Equals(IValue other)
        {
            if (other is NullValue)
            {
                return true;
            }
            return false;
        }

        public bool IsValid()
        {
            return false;
        }

        public object GetValue()
        {
            return null;
        }

        public override string ToString()
        {
            return "null";
        }
    }
}