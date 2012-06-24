/*
 *  DBLoader.cs
 *
 *  Copyright (C) 2010-2012 Kyle Olson, kyle@kyleolson.com
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU General Public License
 *  as published by the Free Software Foundation; either version 2
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 *
 */

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

