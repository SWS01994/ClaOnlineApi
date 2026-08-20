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
    public class CircularService
    {
        private readonly CircularRepository _repo;

        public CircularService()
        {
            _repo = new CircularRepository();
        }

        // ---------------- GET ONE ----------------
        public ApiResponse<CircularDetailResponse> Get(int id)
        {
            try
            {
                if (id <= 0)
                {
                    List<string> errs = new List<string>();
                    errs.Add("A valid id is required.");
                    return ApiResponseFactory.ValidationError<CircularDetailResponse>(errs);
                }

                DataSet ds = _repo.Execute("GETBYID", new SqlParameter[]
                {
                    SqlHelper.InParam("@id", SqlDbType.Int, id)
                });

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    return ApiResponseFactory.NotFound<CircularDetailResponse>(Messages.NotFound);
                }

                CircularDetailResponse payload = new CircularDetailResponse();
                payload.Circular = _repo.MapCircular(ds.Tables[0].Rows[0]);

                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    payload.Content = _repo.MapCircularData(ds.Tables[1].Rows[0]);
                }

                return ApiResponseFactory.Success<CircularDetailResponse>(payload);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CircularService.Get");
                return ApiResponseFactory.Error<CircularDetailResponse>(Messages.ServerError);
            }
        }

        // ---------------- SEARCH ----------------
        public ApiResponse<PagedResult<Circular>> Search(CircularSearchRequest request)
        {
            try
            {
                if (request == null) request = new CircularSearchRequest();
                request.Normalize();

                if (request.FromDate.HasValue && request.ToDate.HasValue
                    && request.FromDate.Value > request.ToDate.Value)
                {
                    List<string> errs = new List<string>();
                    errs.Add("FromDate cannot be later than ToDate.");
                    return ApiResponseFactory.ValidationError<PagedResult<Circular>>(errs);
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

                PagedResult<Circular> result = new PagedResult<Circular>();
                result.Items = new List<Circular>();
                result.PageNumber = request.PageNumber;
                result.PageSize = request.PageSize;
                result.TotalCount = 0;

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    result.TotalCount = ds.Tables[0].Rows[0].GetInt("TotalCount");

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        result.Items.Add(_repo.MapCircular(row));
                    }
                }

                return ApiResponseFactory.Success<PagedResult<Circular>>(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CircularService.Search");
                return ApiResponseFactory.Error<PagedResult<Circular>>(Messages.ServerError);
            }
        }

        // ---------------- SAVE ----------------
        public ApiResponse<int> Save(SaveCircularRequest request)
        {
            try
            {
                ValidationResult v = ValidateSave(request);
                if (!v.IsValid)
                {
                    return ApiResponseFactory.ValidationError<int>(v.Errors);
                }

                int newId = _repo.ExecuteReturnNewId("SAVE", BuildSaveParams(request));
                return ApiResponseFactory.Success<int>(newId, "Circular saved successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CircularService.Save");
                return ApiResponseFactory.Error<int>(Messages.ServerError);
            }
        }

        // ---------------- UPDATE ----------------
        public ApiResponse<bool> Update(UpdateCircularRequest request)
        {
            try
            {
                if (request == null || request.id <= 0)
                {
                    List<string> errs = new List<string>();
                    errs.Add("A valid id is required.");
                    return ApiResponseFactory.ValidationError<bool>(errs);
                }

                ValidationResult v = ValidateSave(request);
                if (!v.IsValid)
                {
                    return ApiResponseFactory.ValidationError<bool>(v.Errors);
                }

                if (!Exists(request.id))
                {
                    return ApiResponseFactory.NotFound<bool>(Messages.NotFound);
                }

                SqlParameter[] p = BuildSaveParams(request);
                SqlParameter[] withId = Prepend(p, SqlHelper.InParam("@id", SqlDbType.Int, request.id));

                _repo.ExecuteNonQuery("UPDATE", withId);
                return ApiResponseFactory.Success<bool>(true, "Circular updated successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CircularService.Update");
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
                    errs.Add("A valid id is required.");
                    return ApiResponseFactory.ValidationError<bool>(errs);
                }

                if (!Exists(id))
                {
                    return ApiResponseFactory.NotFound<bool>(Messages.NotFound);
                }

                _repo.ExecuteNonQuery("DELETE", new SqlParameter[]
                {
                    SqlHelper.InParam("@id", SqlDbType.Int, id)
                });

                return ApiResponseFactory.Success<bool>(true, "Circular deleted successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CircularService.Delete");
                return ApiResponseFactory.Error<bool>(Messages.ServerError);
            }
        }

        // ================= private helpers =================

        private bool Exists(int id)
        {
            DataSet ds = _repo.Execute("GETBYID", new SqlParameter[]
            {
                SqlHelper.InParam("@id", SqlDbType.Int, id)
            });
            return ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0;
        }

        private ValidationResult ValidateSave(SaveCircularRequest r)
        {
            ValidationResult v = new ValidationResult();
            if (r == null)
            {
                v.Add("Request body is required.");
                return v;
            }

            // Adjust to your real mandatory fields.
            v.AddIfEmpty(r.CircNo, "CircNo");
            v.AddIfEmpty(r.IssueYear, "IssueYear");

            return v;
        }

        private SqlParameter[] BuildSaveParams(SaveCircularRequest r)
        {
            return new SqlParameter[]
            {
                SqlHelper.InParam("@Title",           SqlDbType.NVarChar, r.Title),
                SqlHelper.InParam("@CircNo",          SqlDbType.NVarChar, r.CircNo),
                SqlHelper.InParam("@CircDate",        SqlDbType.NVarChar, r.CircDate),
                SqlHelper.InParam("@Category",        SqlDbType.NVarChar, r.Category),
                SqlHelper.InParam("@Summary",         SqlDbType.NVarChar, r.Summary),
                SqlHelper.InParam("@Type",            SqlDbType.NVarChar, r.Type),
                SqlHelper.InParam("@Subject",         SqlDbType.NVarChar, r.Subject),
                SqlHelper.InParam("@Books",           SqlDbType.NVarChar, r.Books),
                SqlHelper.InParam("@Vol",             SqlDbType.NVarChar, r.Vol),
                SqlHelper.InParam("@Sections",        SqlDbType.NVarChar, r.Sections),
                SqlHelper.InParam("@IssueMonth",      SqlDbType.NVarChar, r.IssueMonth),
                SqlHelper.InParam("@IssueYear",       SqlDbType.NVarChar, r.IssueYear),
                SqlHelper.InParam("@FileName",        SqlDbType.NVarChar, r.FileName),
                SqlHelper.InParam("@Citation",        SqlDbType.NVarChar, r.Citation),
                SqlHelper.InParam("@Equi_Citation",   SqlDbType.NVarChar, r.Equi_Citation),
                SqlHelper.InParam("@Equivalent_SCL",  SqlDbType.NVarChar, r.Equivalent_SCL),
                SqlHelper.InParam("@Equivalent_Comp_Cas",SqlDbType.NVarChar, r.Equivalent_Comp_Cas),
                SqlHelper.InParam("@Equivalent_CLJ",  SqlDbType.NVarChar, r.Equivalent_CLJ),
                SqlHelper.InParam("@CircDate_new",    SqlDbType.DateTime, r.CircDate_new.HasValue ? (object)r.CircDate_new.Value : null),
                SqlHelper.InParam("@ShowOnHome",      SqlDbType.Bit,      r.ShowOnHome),
                SqlHelper.InParam("@Filetext",        SqlDbType.NVarChar, r.Filetext),
                SqlHelper.InParam("@FileHTML",        SqlDbType.NVarChar, r.FileHTML),
                SqlHelper.InParam("@Srn",             SqlDbType.Int,      r.Srn)
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