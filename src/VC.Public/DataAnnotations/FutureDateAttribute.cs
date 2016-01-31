using System;
using System.ComponentModel.DataAnnotations;

namespace VC.Public.DataAnnotations
{
	[AttributeUsage(AttributeTargets.Property)]
	public class FutureDateAttribute : ValidationAttribute
	{
		public override bool IsValid(object value)
		{
			if (value == null)
				return true;

			DateTime dt;
			if (DateTime.TryParse(value.ToString(), out dt))
			{
				return dt.Date > DateTime.Today;
			}

			return true;
		}
	}
}