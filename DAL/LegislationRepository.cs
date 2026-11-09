using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebAPI.Helpers;
using WebAPI.Models.Entities;

namespace WebAPI.DAL
{
    public class LegislationRepository
    {
        private const string Proc = "dbo.usp_Legislation_Manage";

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

        public Legislation MapLegislation(DataRow row)
        {
            Legislation l = new Legislation();
            l.id = row.GetInt("id");
            l.BookMark = row.GetString("BookMark");
            l.Title = row.GetString("Title");
            l.Headings = row.GetString("Headings");
            l.Category = row.GetString("Category");
            l.Subject = row.GetString("Subject");
            l.Type = row.GetString("Type");
            l.TitleNo = row.GetString("TitleNo");
            l.ChapterHeading = row.GetString("ChapterHeading");
            l.Books = row.GetString("Books");
            l.Vol = row.GetString("Vol");
            l.Sections = row.GetString("Sections");
            l.LegislationNo = row.GetString("Legislation");   // DB column name is 'Legislation'
            l.IssueYear = row.GetNullableInt("IssueYear");
            l.TOCname = row.GetString("TOCname");
            l.Filename = row.GetString("Filename");
            l.InsertedDate = row.GetNullableDate("InsertedDate");
            l.UpdateDate = row.GetNullableDate("UpdateDate");
            l.ShowOnHome = row.GetBool("ShowOnHome");
            l.FileID = row.GetNullableInt("FileID");
            return l;
        }

        public LegislationData MapLegislationData(DataRow row)
        {
            LegislationData d = new LegislationData();
            d.ID = row.GetInt("ID");
            d.Legislation_ID = row.GetInt("Legislation_ID");
            d.FileName = row.GetString("FileName");
            d.Filetext = row.GetString("Filetext");
            d.FileHTML = row.GetString("FileHTML");
            d.Srn = row.GetInt("srn");
            return d;
        }
    }
}