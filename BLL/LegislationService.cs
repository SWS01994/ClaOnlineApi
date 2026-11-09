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
    public class LegislationService
    {
        private readonly LegislationRepository _repo;

        public LegislationService()
        {
            _repo = new LegislationRepository();
        }

        // ---------------- GET ONE ----------------
        public ApiResponse<LegislationDetailResponse> Get(int id)
        {
            try
            {
                if (id <= 0)
                {
                    List<string> errs = new List<string>();
                    errs.Add("A valid id is required.");
                    return ApiResponseFactory.ValidationError<LegislationDetailResponse>(errs);
                }

                DataSet ds = _repo.Execute("GETBYID", new SqlParameter[]
                {
                    SqlHelper.InParam("@id", SqlDbType.Int, id)
                });

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    return ApiResponseFactory.NotFound<LegislationDetailResponse>(Messages.NotFound);
                }

                LegislationDetailResponse payload = new LegislationDetailResponse();
                payload.Legislation = _repo.MapLegislation(ds.Tables[0].Rows[0]);

                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    payload.Content = _repo.MapLegislationData(ds.Tables[1].Rows[0]);
                }

                return ApiResponseFactory.Success<LegislationDetailResponse>(payload);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "LegislationService.Get");
                return ApiResponseFactory.Error<LegislationDetailResponse>(Messages.ServerError);
            }
        }

        // ---------------- SEARCH ----------------
        public ApiResponse<PagedResult<Legislation>> Search(LegislationSearchRequest request)
        {
            try
            {
                if (request == null) request = new LegislationSearchRequest();
                request.Normalize();

                if (request.FromDate.HasValue && request.ToDate.HasValue
                    && request.FromDate.Value > request.ToDate.Value)
                {
                    List<string> errs = new List<string>();
                    errs.Add("FromDate cannot be later than ToDate.");
                    return ApiResponseFactory.ValidationError<PagedResult<Legislation>>(errs);
                }

                DataSet ds = _repo.Execute("SEARCH", new SqlParameter[]
                {
                    //SqlHelper.InParam("@Category",   SqlDbType.NVarChar, request.Category),
                    //SqlHelper.InParam("@Subject",    SqlDbType.NVarChar, request.Subject),
                    //SqlHelper.InParam("@IssueYear",  SqlDbType.Int,      request.IssueYear.HasValue ? (object)request.IssueYear.Value : null),
                    //SqlHelper.InParam("@ShowOnHome", SqlDbType.Bit,      request.ShowOnHome.HasValue ? (object)request.ShowOnHome.Value : null),
                    SqlHelper.InParam("@FromDate",   SqlDbType.Date,     request.FromDate.HasValue ? (object)request.FromDate.Value : null),
                    SqlHelper.InParam("@ToDate",     SqlDbType.Date,     request.ToDate.HasValue ? (object)request.ToDate.Value : null),
                    SqlHelper.InParam("@PageNumber", SqlDbType.Int,      request.PageNumber),
                    SqlHelper.InParam("@PageSize",   SqlDbType.Int,      request.PageSize)
                });

                PagedResult<Legislation> result = new PagedResult<Legislation>();
                result.Items = new List<Legislation>();
                result.PageNumber = request.PageNumber;
                result.PageSize = request.PageSize;
                result.TotalCount = 0;

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    result.TotalCount = ds.Tables[0].Rows[0].GetInt("TotalCount");

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        result.Items.Add(_repo.MapLegislation(row));
                    }
                }

                return ApiResponseFactory.Success<PagedResult<Legislation>>(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "LegislationService.Search");
                return ApiResponseFactory.Error<PagedResult<Legislation>>(Messages.ServerError);
            }
        }

        // ---------------- SAVE ----------------
        public ApiResponse<int> Save(SaveLegislationRequest request)
        {
            try
            {
                ValidationResult v = ValidateSave(request);
                if (!v.IsValid)
                {
                    return ApiResponseFactory.ValidationError<int>(v.Errors);
                }

                int newId = _repo.ExecuteReturnNewId("SAVE", BuildSaveParams(request));
                return ApiResponseFactory.Success<int>(newId, "Legislation saved successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "LegislationService.Save");
                return ApiResponseFactory.Error<int>(Messages.ServerError);
            }
        }

        // ---------------- UPDATE ----------------
        public ApiResponse<bool> Update(UpdateLegislationRequest request)
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
                return ApiResponseFactory.Success<bool>(true, "Legislation updated successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "LegislationService.Update");
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

                return ApiResponseFactory.Success<bool>(true, "Legislation deleted successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "LegislationService.Delete");
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

        private ValidationResult ValidateSave(SaveLegislationRequest r)
        {
            ValidationResult v = new ValidationResult();
            if (r == null)
            {
                v.Add("Request body is required.");
                return v;
            }

            // Adjust to your real mandatory fields.
            v.AddIfEmpty(r.Title, "Title");

            if (r.Title != null && r.Title.Length > 2000)
                v.Add("Title must not exceed 2000 characters.");

            return v;
        }

        private SqlParameter[] BuildSaveParams(SaveLegislationRequest r)
        {
            return new SqlParameter[]
            {
                SqlHelper.InParam("@BookMark",       SqlDbType.NVarChar, r.BookMark),
                SqlHelper.InParam("@Title",          SqlDbType.NVarChar, r.Title),
                SqlHelper.InParam("@Headings",       SqlDbType.NVarChar, r.Headings),
                SqlHelper.InParam("@Category",       SqlDbType.NVarChar, r.Category),
                SqlHelper.InParam("@Subject",        SqlDbType.NVarChar, r.Subject),
                SqlHelper.InParam("@Type",           SqlDbType.NVarChar, r.Type),
                SqlHelper.InParam("@TitleNo",        SqlDbType.NVarChar, r.TitleNo),
                SqlHelper.InParam("@ChapterHeading", SqlDbType.NVarChar, r.ChapterHeading),
                SqlHelper.InParam("@Books",          SqlDbType.NVarChar, r.Books),
                SqlHelper.InParam("@Vol",            SqlDbType.NVarChar, r.Vol),
                SqlHelper.InParam("@Sections",       SqlDbType.NVarChar, r.Sections),
                SqlHelper.InParam("@Legislation",    SqlDbType.NVarChar, r.LegislationNo),  // maps to DB column 'Legislation'
                SqlHelper.InParam("@IssueYear",      SqlDbType.Int,      r.IssueYear.HasValue ? (object)r.IssueYear.Value : null),
                SqlHelper.InParam("@TOCname",        SqlDbType.NVarChar, r.TOCname),
                SqlHelper.InParam("@Filename",       SqlDbType.NVarChar, r.Filename),
                SqlHelper.InParam("@ShowOnHome",     SqlDbType.Bit,      r.ShowOnHome),
                SqlHelper.InParam("@FileID",         SqlDbType.Int,      r.FileID.HasValue ? (object)r.FileID.Value : null),
                SqlHelper.InParam("@Filetext",       SqlDbType.NVarChar, r.Filetext),
                SqlHelper.InParam("@FileHTML",       SqlDbType.NVarChar, r.FileHTML),
                SqlHelper.InParam("@Srn",            SqlDbType.Int,      r.Srn)
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