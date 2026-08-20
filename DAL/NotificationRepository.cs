using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebAPI.Helpers;
using WebAPI.Models.Entities;

namespace WebAPI.DAL
{
    public class NotificationRepository
    {
        private const string Proc = "dbo.usp_Notification_Manage";

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

        public Notification MapNotification(DataRow row)
        {
            Notification n = new Notification();
            n.Id = row.GetInt("Id");
            n.Title = row.GetString("Title");
            n.FileNo = row.GetString("FileNo");
            n.NotificationDate = row.GetString("NotificationDate");
            n.Type = row.GetString("Type");
            n.NotificationNo = row.GetString("NotificationNo");
            n.Summary = row.GetString("Summary");
            n.Category = row.GetString("Category");
            n.Subject = row.GetString("Subject");
            n.Books = row.GetString("Books");
            n.Vol = row.GetString("Vol");
            n.Sections = row.GetString("Sections");
            n.IssueMonth = row.GetString("IssueMonth");
            n.IssueYear = row.GetString("IssueYear");
            n.FileName = row.GetString("FileName");
            n.Citation = row.GetString("Citation");
            n.Equi_Citation = row.GetString("Equi_Citation");
            n.Equivalent_SCL = row.GetString("Equivalent_SCL");
            n.Equivalent_Comp_Cas = row.GetString("Equivalent_Comp_Cas");
            n.Equivalent_CLJ = row.GetString("Equivalent_CLJ");
            n.InsertedDate = row.GetNullableDate("InsertedDate");
            n.NotificationDate_new = row.GetNullableDate("NotificationDate_new");
            n.UpdateDate = row.GetNullableDate("UpdateDate");
            n.ShowOnHome = row.GetBool("ShowOnHome");
            return n;
        }

        public NotificationData MapNotificationData(DataRow row)
        {
            NotificationData d = new NotificationData();
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