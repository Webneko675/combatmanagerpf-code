using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Mono.Data.Sqlite;

namespace CombatManager
{
	public class RowsRet : IEnumerable<Row>
	{
		public List<Row> Rows = new List<Row>();

		public Row _Headers;
		public Dictionary<String, int> _ColumnIndexes = new Dictionary<String, int>();

		public RowsRet()
		{

		}

		public RowsRet(SqliteDataReader rd)
		{
			if (rd.Read())
			{
				// Read the first record to retrieve the field names
				_Headers = new Row(this);

				for (int i = 0; i < rd.FieldCount; i++)
				{
					_ColumnIndexes[rd.GetName(i)] = i;
					_Headers.Cols.Add(rd.GetName(i));
				}

				ReadRow(rd);

				while (rd.Read())
					ReadRow(rd);
			}
		}

		private void ReadRow(SqliteDataReader rd)
		{
			Row row = new Row(this);
			for (int i = 0; i < rd.FieldCount; i++)
				row.Cols.Add(rd[i].ToString());

			Rows.Add(row);
		}

		public Row Headers
		{
			get
			{
				return _Headers;
			}
			set
			{
				if (_Headers != value)
				{
					_Headers = value;
					_ColumnIndexes = null;
				}
			}
		}

		public int ColumnIndex(String name)
		{
			if (_ColumnIndexes == null)
			{
				_ColumnIndexes = new Dictionary<string, int>(new RowInsensitiveComparer());

				for (int i = 0; i < Headers.Cols.Count; i++)
				{
					_ColumnIndexes.Add(Headers.Cols[i], i);
				}
			}

			int column = -1;

			_ColumnIndexes.TryGetValue(name, out column);

			return column;
		}

		public bool HasColumn(string name)
		{
			return (ColumnIndex(name) != -1);
		}

		public string ViewableString
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				int[] colSize = new int[Rows[0].Cols.Count];

				foreach (Row row in Rows)
				{
					int i = 0;
					foreach (string col in row.Cols)
					{
						colSize[i] = Math.Min(Math.Max(col.Length,
							colSize[i]), 40);
						i++;
					}
				}

				int j = 0;
				foreach (Row row in Rows)
				{
					List<string> toPrint = new List<string>();
					bool needPrint = true;

					foreach (string col in row.Cols)
					{
						toPrint.Add(col);
					}

					while (needPrint)
					{
						List<string> next = new List<string>();
						StringBuilder line = new StringBuilder();
						needPrint = false;

						for (int i = 0; i < toPrint.Count; i++)
						{
							if (toPrint[i].Length > colSize[i])
							{
								line.Append(toPrint[i].Substring(
									0, colSize[i]));
								line.Append(" ");
								needPrint = true;
								next.Add(" " + toPrint[i].Substring(
									colSize[i]));
							}
							else
							{
								next.Add("");
								line.Append(toPrint[i]);
								line.Append(new string(' ',
									colSize[i] - toPrint[i].Length + 1));
							}
						}
						toPrint = next;
						sb.AppendLine(line.ToString().TrimEnd(' '));
					}

					if (j == 0)
					{
						for (int col = 0; col < row.Cols.Count; col++)
						{
							sb.Append(new string('-', colSize[col]));
							if (col < row.Cols.Count - 1)
							{
								sb.Append(" ");
							}
						}
						sb.AppendLine();
					}
					j++;
				}

				return sb.ToString();
			}
		}

		public IEnumerator<Row> GetEnumerator()
		{
			return Rows.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Rows.GetEnumerator();
		}
	}
}

