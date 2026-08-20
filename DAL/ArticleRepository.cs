using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebAPI.Helpers;
using WebAPI.Models.Entities;

namespace WebAPI.DAL
{
    public class ArticleRepository
    {
        private const string Proc = "dbo.usp_Article_Manage";

        // ---- generic executors: flag is supplied by the caller (BLL) ----

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

        // ---- mappers: DataRow -> entity ----

        public Article MapArticle(DataRow row)
        {
            Article a = new Article();
            a.ID = row.GetInt("ID");
            a.Title = row.GetString("Title");
            a.Author = row.GetString("Author");
            a.Summary = row.GetString("Summary");
            a.Category = row.GetString("Category");
            a.Subject = row.GetString("Subject");
            a.Vol = row.GetString("Vol");
            a.Topics = row.GetString("Topics");
            a.Books = row.GetString("Books");
            a.Sections = row.GetString("Sections");
            a.IssueMonth = row.GetString("IssueMonth");
            a.IssueYear = row.GetString("IssueYear");
            a.FileName = row.GetString("FileName");
            a.InsertedDate = row.GetNullableDate("InsertedDate");
            a.UpdateDate = row.GetNullableDate("UpdateDate");
            a.ShowOnHome = row.GetBool("ShowOnHome");
            a.OrderNumber = row.GetInt("OrderNumber");
            return a;
        }

        public ArticleData MapArticleData(DataRow row)
        {
            ArticleData d = new ArticleData();
            d.ID = row.GetInt("ID");
            d.Article_ID = row.GetInt("Article_ID");
            d.FileName = row.GetString("FileName");
            d.Filetext = row.GetString("Filetext");
            d.FileHTML = row.GetString("FileHTML");
            d.Srn = row.GetInt("srn");
            return d;
        }
    }
}