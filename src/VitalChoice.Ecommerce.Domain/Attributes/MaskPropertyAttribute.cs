using System;
using System.Text;

namespace VitalChoice.Ecommerce.Domain.Attributes
{
    public abstract class ValueMasker
    {
        public virtual char MaskCharacter => 'X';

        public unsafe string MaskArea(string value, int startIndex, int length)
        {
            if (value == null)
                return null;

            string result = new string(value.ToCharArray());

            if (length < 0)
                length = 0;

            if (startIndex + length > result.Length)
                length = result.Length - startIndex;

            fixed (char* val = result)
            {
                for (int i = 0; i < length; i++)
                {
                    val[i + startIndex] = MaskCharacter;
                }
            }
            return result;
        }

        public abstract string MaskValue(string value);

        public virtual bool IsMasked(string value)
        {
            return value?.Contains(MaskCharacter.ToString()) ?? true;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class MaskPropertyAttribute : Attribute
    {
        public string Name { get; }

        public Type Masker { get; }

        public MaskPropertyAttribute(string name, Type masker)
        {
            Masker = masker;
            Name = name;
        }
    }
}