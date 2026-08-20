using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebAPI.Common;
using WebAPI.DAL;
using WebAPI.Helpers;
using WebAPI.Models.Entities;
using WebAPI.Models.Requests;
using WebAPI.Models.Responses;

namespace WebAPI.BLL
{
    public class ArticleService
    {
        private readonly ArticleRepository _repo;

        public ArticleService()
        {
            _repo = new ArticleRepository();
        }

        // ---------------- GET ONE (master + detail) ----------------
        public ApiResponse<ArticleDetailResponse> Get(int id)
        {
            try
            {
                if (id <= 0)
                {
                    List<string> errs = new List<string>();
                    errs.Add("A valid ID is required.");
                    return ApiResponseFactory.ValidationError<ArticleDetailResponse>(errs);
                }

                DataSet ds = _repo.Execute("GETBYID", new SqlParameter[]
                {
                    SqlHelper.InParam("@ID", SqlDbType.Int, id)
                });

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    return ApiResponseFactory.NotFound<ArticleDetailResponse>(Messages.NotFound);
                }

                ArticleDetailResponse payload = new ArticleDetailResponse();
                payload.Article = _repo.MapArticle(ds.Tables[0].Rows[0]);

                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    payload.Content = _repo.MapArticleData(ds.Tables[1].Rows[0]);
                }

                return ApiResponseFactory.Success<ArticleDetailResponse>(payload);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ArticleService.Get");
                return ApiResponseFactory.Error<ArticleDetailResponse>(Messages.ServerError);
            }
        }

        // ---------------- SEARCH / LIST (paged) ----------------
        public ApiResponse<PagedResult<Article>> Search(ArticleSearchRequest request)
        {
            try
            {
                if (request == null) request = new ArticleSearchRequest();
                request.Normalize();

                if (request.FromDate.HasValue && request.ToDate.HasValue
                    && request.FromDate.Value > request.ToDate.Value)
                {
                    List<string> errs = new List<string>();
                    errs.Add("FromDate cannot be later than ToDate.");
                    return ApiResponseFactory.ValidationError<PagedResult<Article>>(errs);
                }

                DataSet ds = _repo.Execute("SEARCH", new SqlParameter[]
                {
                    //SqlHelper.InParam("@Category",   SqlDbType.NVarChar, request.Category),
                    //SqlHelper.InParam("@IssueYear",  SqlDbType.NVarChar, request.IssueYear),
                    //SqlHelper.InParam("@ShowOnHome", SqlDbType.Bit,      request.ShowOnHome.HasValue ? (object)request.ShowOnHome.Value : null),
                    SqlHelper.InParam("@FromDate",   SqlDbType.Date,     request.FromDate.HasValue ? (object)request.FromDate.Value : null),
                    SqlHelper.InParam("@ToDate",     SqlDbType.Date,     request.ToDate.HasValue ? (object)request.ToDate.Value : null),
                    SqlHelper.InParam("@PageNumber", SqlDbType.Int,      request.PageNumber),
                    SqlHelper.InParam("@PageSize",   SqlDbType.Int,      request.PageSize)
                });

                PagedResult<Article> result = new PagedResult<Article>();
                result.Items = new List<Article>();
                result.PageNumber = request.PageNumber;
                result.PageSize = request.PageSize;
                result.TotalCount = 0;

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    result.TotalCount = ds.Tables[0].Rows[0].GetInt("TotalCount");

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        result.Items.Add(_repo.MapArticle(row));
                    }
                }

                return ApiResponseFactory.Success<PagedResult<Article>>(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ArticleService.Search");
                return ApiResponseFactory.Error<PagedResult<Article>>(Messages.ServerError);
            }
        }

        // ---------------- SAVE (insert master + detail) ----------------
        public ApiResponse<int> Save(SaveArticleRequest request)
        {
            try
            {
                ValidationResult v = ValidateSave(request);
                if (!v.IsValid)
                {
                    return ApiResponseFactory.ValidationError<int>(v.Errors);
                }

                Article a = MapToArticle(request);
                ArticleData d = MapToArticleData(request);

                int newId = _repo.ExecuteReturnNewId("SAVE", BuildSaveParams(a, d));

                return ApiResponseFactory.Success<int>(newId, "Article saved successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ArticleService.Save");
                return ApiResponseFactory.Error<int>(Messages.ServerError);
            }
        }

        // ---------------- UPDATE (master + detail) ----------------
        public ApiResponse<bool> Update(UpdateArticleRequest request)
        {
            try
            {
                if (request == null || request.ID <= 0)
                {
                    List<string> errs = new List<string>();
                    errs.Add("A valid ID is required.");
                    return ApiResponseFactory.ValidationError<bool>(errs);
                }

                ValidationResult v = ValidateSave(request);
                if (!v.IsValid)
                {
                    return ApiResponseFactory.ValidationError<bool>(v.Errors);
                }

                // Existence check so a missing ID reports "not found"
                // instead of silently affecting zero rows.
                if (!Exists(request.ID))
                {
                    return ApiResponseFactory.NotFound<bool>(Messages.NotFound);
                }

                Article a = MapToArticle(request);
                a.ID = request.ID;
                ArticleData d = MapToArticleData(request);

                SqlParameter[] p = BuildSaveParams(a, d);
                SqlParameter[] withId = Prepend(p, SqlHelper.InParam("@ID", SqlDbType.Int, request.ID));

                _repo.ExecuteNonQuery("UPDATE", withId);

                return ApiResponseFactory.Success<bool>(true, "Article updated successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ArticleService.Update");
                return ApiResponseFactory.Error<bool>(Messages.ServerError);
            }
        }

        // ---------------- DELETE (hard delete) ----------------
        public ApiResponse<bool> Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    List<string> errs = new List<string>();
                    errs.Add("A valid ID is required.");
                    return ApiResponseFactory.ValidationError<bool>(errs);
                }

                if (!Exists(id))
                {
                    return ApiResponseFactory.NotFound<bool>(Messages.NotFound);
                }

                _repo.ExecuteNonQuery("DELETE", new SqlParameter[]
                {
                    SqlHelper.InParam("@ID", SqlDbType.Int, id)
                });

                return ApiResponseFactory.Success<bool>(true, "Article deleted successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ArticleService.Delete");
                return ApiResponseFactory.Error<bool>(Messages.ServerError);
            }
        }

        // ================= private helpers =================

        private bool Exists(int id)
        {
            DataSet ds = _repo.Execute("GETBYID", new SqlParameter[]
            {
                SqlHelper.InParam("@ID", SqlDbType.Int, id)
            });
            return ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0;
        }

        private ValidationResult ValidateSave(SaveArticleRequest r)
        {
            ValidationResult v = new ValidationResult();
            if (r == null)
            {
                v.Add("Request body is required.");
                return v;
            }

            v.AddIfEmpty(r.Title, "Title");
            v.AddIfEmpty(r.IssueYear, "IssueYear");

            // Length guards matching the column sizes, so we fail early
            // with a clear message instead of a SQL truncation error.
            if (r.Title != null && r.Title.Length > 2000)
                v.Add("Title must not exceed 2000 characters.");
            if (r.Summary != null && r.Summary.Length > 4000)
                v.Add("Summary must not exceed 4000 characters.");
            if (r.Author != null && r.Author.Length > 255)
                v.Add("Author must not exceed 255 characters.");

            return v;
        }

        private Article MapToArticle(SaveArticleRequest r)
        {
            Article a = new Article();
            a.Title = r.Title;
            a.Author = r.Author;
            a.Summary = r.Summary;
            a.Category = r.Category;
            a.Subject = r.Subject;
            a.Vol = r.Vol;
            a.Topics = r.Topics;
            a.Books = r.Books;
            a.Sections = r.Sections;
            a.IssueMonth = r.IssueMonth;
            a.IssueYear = r.IssueYear;
            a.FileName = r.FileName;
            a.ShowOnHome = r.ShowOnHome;
            a.OrderNumber = r.OrderNumber;
            return a;
        }

        private ArticleData MapToArticleData(SaveArticleRequest r)
        {
            ArticleData d = new ArticleData();
            d.FileName = r.FileName;
            d.Filetext = r.Filetext;
            d.FileHTML = r.FileHTML;
            d.Srn = r.Srn;
            return d;
        }

        private SqlParameter[] BuildSaveParams(Article a, ArticleData d)
        {
            return new SqlParameter[]
            {
                SqlHelper.InParam("@Title",      SqlDbType.NVarChar, a.Title),
                SqlHelper.InParam("@Author",     SqlDbType.NVarChar, a.Author),
                SqlHelper.InParam("@Summary",    SqlDbType.NVarChar, a.Summary),
                SqlHelper.InParam("@Category",   SqlDbType.NVarChar, a.Category),
                SqlHelper.InParam("@Subject",    SqlDbType.NVarChar, a.Subject),
                SqlHelper.InParam("@Vol",        SqlDbType.NVarChar, a.Vol),
                SqlHelper.InParam("@Topics",     SqlDbType.NVarChar, a.Topics),
                SqlHelper.InParam("@Books",      SqlDbType.NVarChar, a.Books),
                SqlHelper.InParam("@Sections",   SqlDbType.NVarChar, a.Sections),
                SqlHelper.InParam("@IssueMonth", SqlDbType.NVarChar, a.IssueMonth),
                SqlHelper.InParam("@IssueYear",  SqlDbType.NVarChar, a.IssueYear),
                SqlHelper.InParam("@FileName",   SqlDbType.NVarChar, a.FileName),
                SqlHelper.InParam("@ShowOnHome", SqlDbType.Bit,      a.ShowOnHome),
                SqlHelper.InParam("@OrderNumber",SqlDbType.Int,      a.OrderNumber),
                SqlHelper.InParam("@Filetext",   SqlDbType.NVarChar, d.Filetext),
                SqlHelper.InParam("@FileHTML",   SqlDbType.NVarChar, d.FileHTML),
                SqlHelper.InParam("@Srn",        SqlDbType.Int,      d.Srn)
            };
        }

        private static SqlParameter[] Prepend(SqlParameter[] arr, SqlParameter first)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(first);
            list.AddRange(arr);
            return list.ToArray();
        }
    }
}