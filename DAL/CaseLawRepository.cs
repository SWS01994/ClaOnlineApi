using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebAPI.Helpers;
using WebAPI.Models.Entities;

namespace WebAPI.DAL
{
    public class CaseLawRepository
    {
        private const string Proc = "dbo.usp_CaseLaw_Manage";

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

        public CaseLaw MapCaseLaw(DataRow row)
        {
            CaseLaw c = new CaseLaw();
            c.id = row.GetInt("id");
            c.FileName = row.GetString("FileName");
            c.Sections = row.GetString("Sections");
            c.Versus = row.GetString("Versus");
            c.Citation = row.GetString("Citation");
            c.Equi_Citation = row.GetString("Equi_Citation");
            c.CaseNo = row.GetString("CaseNo");
            c.Judge = row.GetString("Judge");
            c.Date_of_Judgement = row.GetString("Date_of_Judgement");
            c.HeadNote = row.GetString("HeadNote");
            c.AdvocateName = row.GetString("AdvocateName");
            c.CasesReffered = row.GetString("CasesReffered");
            c.Category = row.GetString("Category");
            c.Type = row.GetString("Type");
            c.Subject = row.GetString("Subject");
            c.CitYear = row.GetString("CitYear");
            c.CourtName = row.GetString("CourtName");
            c.Vol = row.GetString("Vol");
            c.PageNO = row.GetString("PageNO");
            c.Books = row.GetString("Books");
            c.BusinessSupp = row.GetString("BusinessSupp");
            c.IssueMonth = row.GetString("IssueMonth");
            c.IssueYear = row.GetString("IssueYear");
            c.InsertedDate = row.GetNullableDate("InsertedDate");
            c.Judgement_date_new = row.GetNullableDate("Judgement_date_new");
            c.Equivalent_SCL = row.GetString("Equivalent_SCL");
            c.Equivalent_Comp_Cas = row.GetString("Equivalent_Comp_Cas");
            c.Equivalent_CLJ = row.GetString("Equivalent_CLJ");
            c.UpdateDate = row.GetNullableDate("UpdateDate");
            c.ShowOnHome = row.GetBool("ShowOnHome");
            return c;
        }

        public CaseLawData MapCaseLawData(DataRow row)
        {
            CaseLawData d = new CaseLawData();
            d.ID = row.GetInt("ID");
            d.CaseLawID = row.GetInt("CaseLawID");
            d.FileName = row.GetString("FileName");
            d.Filetext = row.GetString("Filetext");
            d.FileHTML = row.GetString("FileHTML");
            d.Srn = row.GetInt("srn");
            return d;
        }
    }
}