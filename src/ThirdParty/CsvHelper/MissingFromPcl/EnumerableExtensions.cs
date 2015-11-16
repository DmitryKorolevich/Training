using System;
using System.Linq;

namespace CsvHelper.MissingFromPcl
{
	internal static class EnumerableExtensions
	{
		public static bool All( this string s, Func<char, bool> predicate )
		{
			return s.Cast<char>().All( predicate );
		}
	}
}
