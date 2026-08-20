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
    public class QueryService
    {
        private readonly QueryRepository _repo;

        public QueryService()
        {
            _repo = new QueryRepository();
        }

        // ---------------- GET ONE ----------------
        public ApiResponse<QueryDetailResponse> Get(int id)
        {
            try
            {
                if (id <= 0)
                {
                    List<string> errs = new List<string>();
                    errs.Add("A valid ID is required.");
                    return ApiResponseFactory.ValidationError<QueryDetailResponse>(errs);
                }

                DataSet ds = _repo.Execute("GETBYID", new SqlParameter[]
                {
                    SqlHelper.InParam("@ID", SqlDbType.Int, id)
                });

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    return ApiResponseFactory.NotFound<QueryDetailResponse>(Messages.NotFound);
                }

                QueryDetailResponse payload = new QueryDetailResponse();
                payload.Query = _repo.MapQuery(ds.Tables[0].Rows[0]);

                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    payload.Content = _repo.MapQueryData(ds.Tables[1].Rows[0]);
                }

                return ApiResponseFactory.Success<QueryDetailResponse>(payload);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "QueryService.Get");
                return ApiResponseFactory.Error<QueryDetailResponse>(Messages.ServerError);
            }
        }

        // ---------------- SEARCH ----------------
        public ApiResponse<PagedResult<Query>> Search(QuerySearchRequest request)
        {
            try
            {
                if (request == null) request = new QuerySearchRequest();
                request.Normalize();

                if (request.FromDate.HasValue && request.ToDate.HasValue
                    && request.FromDate.Value > request.ToDate.Value)
                {
                    List<string> errs = new List<string>();
                    errs.Add("FromDate cannot be later than ToDate.");
                    return ApiResponseFactory.ValidationError<PagedResult<Query>>(errs);
                }

                DataSet ds = _repo.Execute("SEARCH", new SqlParameter[]
                {
                    SqlHelper.InParam("@FromDate",   SqlDbType.Date,     request.FromDate.HasValue ? (object)request.FromDate.Value : null),
                    SqlHelper.InParam("@ToDate",     SqlDbType.Date,     request.ToDate.HasValue ? (object)request.ToDate.Value : null),
                    SqlHelper.InParam("@PageNumber", SqlDbType.Int,      request.PageNumber),
                    SqlHelper.InParam("@PageSize",   SqlDbType.Int,      request.PageSize)
                });

                PagedResult<Query> result = new PagedResult<Query>();
                result.Items = new List<Query>();
                result.PageNumber = request.PageNumber;
                result.PageSize = request.PageSize;
                result.TotalCount = 0;

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    result.TotalCount = ds.Tables[0].Rows[0].GetInt("TotalCount");

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        result.Items.Add(_repo.MapQuery(row));
                    }
                }

                return ApiResponseFactory.Success<PagedResult<Query>>(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "QueryService.Search");
                return ApiResponseFactory.Error<PagedResult<Query>>(Messages.ServerError);
            }
        }

        // ---------------- SAVE ----------------
        public ApiResponse<int> Save(SaveQueryRequest request)
        {
            try
            {
                ValidationResult v = ValidateSave(request);
                if (!v.IsValid)
                {
                    return ApiResponseFactory.ValidationError<int>(v.Errors);
                }

                int newId = _repo.ExecuteReturnNewId("SAVE", BuildSaveParams(request));
                return ApiResponseFactory.Success<int>(newId, "Query saved successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "QueryService.Save");
                return ApiResponseFactory.Error<int>(Messages.ServerError);
            }
        }

        // ---------------- UPDATE ----------------
        public ApiResponse<bool> Update(UpdateQueryRequest request)
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

                if (!Exists(request.ID))
                {
                    return ApiResponseFactory.NotFound<bool>(Messages.NotFound);
                }

                SqlParameter[] p = BuildSaveParams(request);
                SqlParameter[] withId = Prepend(p, SqlHelper.InParam("@ID", SqlDbType.Int, request.ID));

                _repo.ExecuteNonQuery("UPDATE", withId);
                return ApiResponseFactory.Success<bool>(true, "Query updated successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "QueryService.Update");
                return ApiResponseFactory.Error<bool>(Messages.ServerError);
            }
        }

        // ---------------- DELETE ----------------
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

                return ApiResponseFactory.Success<bool>(true, "Query deleted successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "QueryService.Delete");
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

        private ValidationResult ValidateSave(SaveQueryRequest r)
        {
            ValidationResult v = new ValidationResult();
            if (r == null)
            {
                v.Add("Request body is required.");
                return v;
            }

            // Adjust to your real mandatory fields.
            v.AddIfEmpty(r.Title, "Title");
            v.AddIfEmpty(r.IssueYear, "IssueYear");

            if (r.Title != null && r.Title.Length > 4000)
                v.Add("Title must not exceed 4000 characters.");

            return v;
        }

        private SqlParameter[] BuildSaveParams(SaveQueryRequest r)
        {
            return new SqlParameter[]
            {
                SqlHelper.InParam("@Subject",    SqlDbType.NVarChar, r.Subject),
                SqlHelper.InParam("@Topics",     SqlDbType.NVarChar, r.Topics),
                SqlHelper.InParam("@Summary",    SqlDbType.NVarChar, r.Summary),
                SqlHelper.InParam("@Title",      SqlDbType.NVarChar, r.Title),
                SqlHelper.InParam("@Sections",   SqlDbType.NVarChar, r.Sections),
                SqlHelper.InParam("@Vol",        SqlDbType.Int,      r.Vol.HasValue ? (object)r.Vol.Value : null),
                SqlHelper.InParam("@IssueYear",  SqlDbType.NVarChar, r.IssueYear),
                SqlHelper.InParam("@FileName",   SqlDbType.NVarChar, r.FileName),
                SqlHelper.InParam("@Books",      SqlDbType.NVarChar, r.Books),
                SqlHelper.InParam("@ShowOnHome", SqlDbType.Bit,      r.ShowOnHome),
                SqlHelper.InParam("@Filetext",   SqlDbType.NVarChar, r.Filetext),
                SqlHelper.InParam("@FileHTML",   SqlDbType.NVarChar, r.FileHTML),
                SqlHelper.InParam("@Srn",        SqlDbType.Int,      r.Srn)
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