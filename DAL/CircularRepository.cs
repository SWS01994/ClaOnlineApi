using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebAPI.Helpers;
using WebAPI.Models.Entities;

namespace WebAPI.DAL
{
    public class CircularRepository
    {
        private const string Proc = "dbo.usp_Circular_Manage";

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

        public Circular MapCircular(DataRow row)
        {
            Circular c = new Circular();
            c.id = row.GetInt("id");
            c.Title = row.GetString("Title");
            c.CircNo = row.GetString("CircNo");
            c.CircDate = row.GetString("CircDate");
            c.Category = row.GetString("Category");
            c.Summary = row.GetString("Summary");
            c.Type = row.GetString("Type");
            c.Subject = row.GetString("Subject");
            c.Books = row.GetString("Books");
            c.Vol = row.GetString("Vol");
            c.Sections = row.GetString("Sections");
            c.IssueMonth = row.GetString("IssueMonth");
            c.IssueYear = row.GetString("IssueYear");
            c.FileName = row.GetString("FileName");
            c.Citation = row.GetString("Citation");
            c.Equi_Citation = row.GetString("Equi_Citation");
            c.Equivalent_SCL = row.GetString("Equivalent_SCL");
            c.Equivalent_Comp_Cas = row.GetString("Equivalent_Comp_Cas");
            c.Equivalent_CLJ = row.GetString("Equivalent_CLJ");
            c.InsertedDate = row.GetNullableDate("InsertedDate");
            c.CircDate_new = row.GetNullableDate("CircDate_new");
            c.UpdateDate = row.GetNullableDate("UpdateDate");
            c.ShowOnHome = row.GetBool("ShowOnHome");
            return c;
        }

        public CircularData MapCircularData(DataRow row)
        {
            CircularData d = new CircularData();
            d.ID = row.GetInt("ID");
            d.Notification_ID = row.GetInt("Notification_ID");
            d.FileName = row.GetString("FileName");
            d.Filetext = row.GetString("Filetext");
            d.FileHTML = row.GetString("FileHTML");
            d.Srn = row.GetInt("srn");
            return d;
        }
    }
}