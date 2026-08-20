using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebAPI.Helpers;
using WebAPI.Models.Entities;

namespace WebAPI.DAL
{
    public class QueryRepository
    {
        private const string Proc = "dbo.usp_Query_Manage";

        public DataSet Execute(string flag, SqlParameter[] parameters)
        {
            return SqlHelper.ExecuteDataSet(Proc, Combine(flag, parameters));
        }

        public int ExecuteNonQuery(string flag, SqlParameter[] parameters)
        {
            return SqlHelper.ExecuteNonQuery(Proc, Combine(flag, parameters));
        }

        public int ExecuteReturnNewId(string flag, SqlParameter[] parameters)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlHelper.InParam("@Flag", SqlDbType.VarChar, flag));
            if (parameters != null) list.AddRange(parameters);
            list.Add(SqlHelper.OutParam("@NewID", SqlDbType.Int));
            return SqlHelper.ExecuteReturnNewId(Proc, list.ToArray(), "@NewID");
        }

        private static SqlParameter[] Combine(string flag, SqlParameter[] parameters)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlHelper.InParam("@Flag", SqlDbType.VarChar, flag));
            if (parameters != null) list.AddRange(parameters);
            return list.ToArray();
        }

        public Query MapQuery(DataRow row)
        {
            Query q = new Query();
            q.ID = row.GetInt("ID");
            q.Subject = row.GetString("Subject");
            q.Topics = row.GetString("Topics");
            q.Summary = row.GetString("Summary");
            q.Title = row.GetString("Title");
            q.Sections = row.GetString("Sections");
            q.Vol = row.GetNullableInt("Vol");
            q.IssueYear = row.GetString("IssueYear");
            q.FileName = row.GetString("FileName");
            q.Books = row.GetString("Books");
            q.InsertedDate = row.GetNullableDate("InsertedDate");
            q.UpdateDate = row.GetNullableDate("UpdateDate");
            q.ShowOnHome = row.GetBool("ShowOnHome");
            return q;
        }

        public QueryData MapQueryData(DataRow row)
        {
            QueryData d = new QueryData();
            d.ID = row.GetInt("ID");
            d.Query_ID = row.GetInt("Query_ID");
            d.FileName = row.GetString("FileName");
            d.Filetext = row.GetString("Filetext");
            d.FileHTML = row.GetString("FileHTML");
            d.Srn = row.GetInt("srn");
            return d;
        }
    }
}