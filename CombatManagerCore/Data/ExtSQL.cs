using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Mono.Data.Sqlite;

namespace CombatManager
{
	public static class ExtSQL
	{
		public static RowsRet ExecuteCommand(this SqliteConnection sql, string command)
		{
			if (sql.State != ConnectionState.Open)
			    sql.Open();

			using (SqliteCommand cmd = sql.CreateCommand())
			{
				cmd.CommandText = command;

				using (SqliteDataReader rd = cmd.ExecuteReader())
					return new RowsRet(rd);
			}
		}

		public static RowsRet ExecuteCommand(this SqliteConnection sql, string command, object[] param)
		{
			if (sql.State != ConnectionState.Open)
				sql.Open();

			using (SqliteCommand cmd = sql.BuildParameterizedCommand(command, param))
			{
				cmd.CommandText = command;

				foreach (object obj in param)
					cmd.Parameters.Add(CreateParam(obj));

				using (SqliteDataReader rd = cmd.ExecuteReader())
					return new RowsRet(rd);
			}
		}

		public static T ExecuteScalar<T>(this SqliteConnection sql, string command)
		{
			if (sql.State != ConnectionState.Open)
			    sql.Open();

			using (SqliteCommand cmd = sql.CreateCommand())
			{
				cmd.CommandText = command;

				return GetDbField<T>(cmd.ExecuteScalar());
			}
		}

		public static SqliteCommand BuildParameterizedCommand(this SqliteConnection sql, string command, object[] param)
		{
			SqliteCommand cmd = sql.CreateCommand();

			cmd.CommandText = command;

			foreach (object obj in param)
				cmd.Parameters.Add(CreateParam(obj));
			return cmd;
		}

		public static DataSet SelectIntoDataSet(this SqliteConnection sql, string command)
		{
			using (SqliteCommand cmd = sql.CreateCommand())
			{
				cmd.CommandText = command;

				DataSet results = new DataSet("SelectResults");

				SelectIntoDataSet(cmd, results, "Results");
				return results;
			}
		}

		public static DataSet SelectIntoDataSet(this SqliteConnection sql, string command, object[] param)
		{
			using (SqliteCommand cmd = sql.BuildParameterizedCommand(command, param))
			{
				DataSet results = new DataSet("SelectResults");

				SelectIntoDataSet(cmd, results, "Results");
				return results;
			}
		}

		public static void SelectIntoDataSet(SqliteCommand command, DataSet dataSet, string tableName)
		{
			using (SqliteDataAdapter adapter = new SqliteDataAdapter(command))
			{
				// Assume that the connection has already been set up and any transactions have been attached
				if (!dataSet.Tables.Contains(tableName))
					dataSet.Tables.Add(tableName);

				adapter.Fill(dataSet, tableName);
			}
		}

		public static bool DatabaseObjectExists(this SqliteConnection sql, string table)
		{
			RowsRet rows = null;
			rows = sql.ExecuteCommand(
					"SELECT COUNT(*) FROM sqlite_master WHERE name=?", new object[] { table });
			int m_startRow = 1;

			if (rows == null || rows.Rows.Count < ((m_startRow == 0) ? 2 : 1))
			{
				return false;
			}
			else
			{
				int count = int.Parse(rows.Rows[(m_startRow == 0) ? 1 : 0].Cols[0]);

				if (count >= 1)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public static SqliteParameter CreateParam(object obj)
		{
			if (obj is int || obj is int?)
			{
				return new SqliteParameter(DbType.Int32, obj);
			}
			if (obj is string)
			{
				return new SqliteParameter(DbType.String, obj);
			}
			if (obj is Int64 || obj is Int64?)
			{

				return new SqliteParameter(DbType.Int64, obj);
			}
			if (obj is bool || obj is bool?)
			{
				return new SqliteParameter(DbType.Boolean, obj);
			}
			if (obj == null)
			{

				return new SqliteParameter(DbType.String, null);
			}
			else
			{
				return new SqliteParameter(DbType.String, obj.ToString());
			}

		}

		public static T GetDbField<T>(object dbValue)
		{
			if (dbValue == DBNull.Value)
				return default(T);

			return (T)dbValue;
		}

		public static T? GetNullableDbField<T>(object dbValue) where T : struct
		{
			if (dbValue == DBNull.Value)
				return new T?();

			return (T)dbValue;
		}

		#region //****** IDataReader Extensions ******//
		public static bool HasField(this IDataReader reader, string fieldName)
		{
			for (int i = 0; i < reader.FieldCount; i++)
			{
				if (string.Compare(reader.GetName(i), fieldName, true) == 0)
					return true;
			}

			return false;
		}

		public static T GetField<T>(this IDataReader reader, int index)
		{
			object fieldData = reader[index];

			if (fieldData == DBNull.Value)
				return default(T);

			return (T)fieldData;
		}

		public static T? GetNullableField<T>(this IDataReader reader, int index) where T : struct
		{
			object fieldData = reader[index];

			if (fieldData == DBNull.Value)
				return new T?();

			return (T)fieldData;
		}

		public static T GetField<T>(this IDataReader reader, string fieldName)
		{
			object fieldData = reader[fieldName];

			if (fieldData == DBNull.Value)
				return default(T);

			return (T)fieldData;
		}

		public static T? GetNullableField<T>(this IDataReader reader, string fieldName) where T : struct
		{
			object fieldData = reader[fieldName];

			if (fieldData == DBNull.Value)
				return new T?();

			return (T)fieldData;
		}
		#endregion //****** IDataReader Extensions ******//

		#region //****** DataRow Extensions ******//
		public static bool HasField(this DataRow row, string fieldName)
		{
			return row.Table.Columns.Contains(fieldName);
		}

		public static T GetField<T>(this DataRow row, int index)
		{
			object fieldData = row[index];

			if (fieldData == DBNull.Value)
				return default(T);

			return (T)fieldData;
		}

		public static T? GetNullableField<T>(this DataRow row, int index) where T : struct
		{
			object fieldData = row[index];

			if (fieldData == DBNull.Value)
				return new T?();

			return (T)fieldData;
		}

		public static T GetField<T>(this DataRow row, string fieldName)
		{
			object fieldData = row[fieldName];

			if (fieldData == DBNull.Value)
				return default(T);

			return (T)fieldData;
		}

		public static T? GetNullableField<T>(this DataRow row, string fieldName) where T : struct
		{
			object fieldData = row[fieldName];

			if (fieldData == DBNull.Value)
				return new T?();

			return (T)fieldData;
		}
		#endregion //****** DataRow Extensions ******//
	}
}

