using System;
using System.Data;

namespace WebAPI.Helpers
{
    public static class DataRowExtensions
    {
        public static string GetString(this DataRow row, string col)
        {
            if (!row.Table.Columns.Contains(col) || row[col] == DBNull.Value)
                return null;
            return row[col].ToString();
        }

        public static int GetInt(this DataRow row, string col)
        {
            if (!row.Table.Columns.Contains(col) || row[col] == DBNull.Value)
                return 0;
            return Convert.ToInt32(row[col]);
        }

        public static bool GetBool(this DataRow row, string col)
        {
            if (!row.Table.Columns.Contains(col) || row[col] == DBNull.Value)
                return false;
            return Convert.ToBoolean(row[col]);
        }

        public static DateTime? GetNullableDate(this DataRow row, string col)
        {
            if (!row.Table.Columns.Contains(col) || row[col] == DBNull.Value)
                return null;
            return Convert.ToDateTime(row[col]);
        }

        // Worth adding now — several of your tables will need these:
        public static decimal GetDecimal(this DataRow row, string col)
        {
            if (!row.Table.Columns.Contains(col) || row[col] == DBNull.Value)
                return 0m;
            return Convert.ToDecimal(row[col]);
        }

        public static int? GetNullableInt(this DataRow row, string col)
        {
            if (!row.Table.Columns.Contains(col) || row[col] == DBNull.Value)
                return null;
            return Convert.ToInt32(row[col]);
        }
    }
}