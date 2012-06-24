using System;
using System.Collections.Generic;
using System.Linq;

namespace CombatManager
{
	public class Row
	{
		public Row(RowsRet ResultSet)
		{
			this._ResultSet = ResultSet;
		}

		public RowsRet _ResultSet;

		public List<string> Cols = new List<string>();

		public String this[String name]
		{
			get
			{
				string val = null;

				int column = _ResultSet.ColumnIndex(name);

				if (column != -1)
				{
					val = Cols[column];
				}
				return val;
			}
			set
			{

				int column = _ResultSet.ColumnIndex(name);

				if (column != -1)
				{
					Cols[column] = value;
				}
			}
		}

		public bool BoolValue(string value)
		{
			return this[value] == "1";
		}

		public Nullable<int> NullableIntValue(string value)
		{
			Nullable<int> val = null;

			int num;
			if (int.TryParse(this[value], out num))
			{
				val = num;
			}

			return val;
		}

		public int IntValue(string value)
		{
			int num = 0;
			int.TryParse(this[value], out num);
			return num;
		}


	}
}

