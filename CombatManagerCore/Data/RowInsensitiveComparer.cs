using System;
using System.Collections.Generic;
using System.Linq;

namespace CombatManager
{
	public class RowInsensitiveComparer : IComparer<string>, IEqualityComparer<string>
	{
		public int Compare(string a, string b)
		{
			return String.Compare(a, b, true);
		}

		public bool Equals(string a, string b)
		{
			return Compare(a, b) == 0;
		}

		public int GetHashCode(string obj)
		{
			return obj.GetHashCode();
		}
	}
}

