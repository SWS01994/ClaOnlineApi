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
    public class CaseLawService
    {
        private readonly CaseLawRepository _repo;

        public CaseLawService()
        {
            _repo = new CaseLawRepository();
        }

        // ---------------- GET ONE ----------------
        public ApiResponse<CaseLawDetailResponse> Get(int id)
        {
            try
            {
                if (id <= 0)
                {
                    List<string> errs = new List<string>();
                    errs.Add("A valid id is required.");
                    return ApiResponseFactory.ValidationError<CaseLawDetailResponse>(errs);
                }

                DataSet ds = _repo.Execute("GETBYID", new SqlParameter[]
                {
                    SqlHelper.InParam("@id", SqlDbType.Int, id)
                });

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    return ApiResponseFactory.NotFound<CaseLawDetailResponse>(Messages.NotFound);
                }

                CaseLawDetailResponse payload = new CaseLawDetailResponse();
                payload.CaseLaw = _repo.MapCaseLaw(ds.Tables[0].Rows[0]);

                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    payload.Content = _repo.MapCaseLawData(ds.Tables[1].Rows[0]);
                }

                return ApiResponseFactory.Success<CaseLawDetailResponse>(payload);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CaseLawService.Get");
                return ApiResponseFactory.Error<CaseLawDetailResponse>(Messages.ServerError);
            }
        }

        // ---------------- SEARCH ----------------
        public ApiResponse<PagedResult<CaseLaw>> Search(CaseLawSearchRequest request)
        {
            try
            {
                if (request == null) request = new CaseLawSearchRequest();
                request.Normalize();

                if (request.FromDate.HasValue && request.ToDate.HasValue
                    && request.FromDate.Value > request.ToDate.Value)
                {
                    List<string> errs = new List<string>();
                    errs.Add("FromDate cannot be later than ToDate.");
                    return ApiResponseFactory.ValidationError<PagedResult<CaseLaw>>(errs);
                }

                DataSet ds = _repo.Execute("SEARCH", new SqlParameter[]
                {
                    //SqlHelper.InParam("@Category",   SqlDbType.NVarChar, request.Category),
                    //SqlHelper.InParam("@CourtName",  SqlDbType.NVarChar, request.CourtName),
                    //SqlHelper.InParam("@IssueYear",  SqlDbType.NVarChar, request.IssueYear),
                    //SqlHelper.InParam("@ShowOnHome", SqlDbType.Bit,      request.ShowOnHome.HasValue ? (object)request.ShowOnHome.Value : null),
                    SqlHelper.InParam("@FromDate",   SqlDbType.Date,     request.FromDate.HasValue ? (object)request.FromDate.Value : null),
                    SqlHelper.InParam("@ToDate",     SqlDbType.Date,     request.ToDate.HasValue ? (object)request.ToDate.Value : null),
                    SqlHelper.InParam("@PageNumber", SqlDbType.Int,      request.PageNumber),
                    SqlHelper.InParam("@PageSize",   SqlDbType.Int,      request.PageSize)
                });

                PagedResult<CaseLaw> result = new PagedResult<CaseLaw>();
                result.Items = new List<CaseLaw>();
                result.PageNumber = request.PageNumber;
                result.PageSize = request.PageSize;
                result.TotalCount = 0;

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    result.TotalCount = ds.Tables[0].Rows[0].GetInt("TotalCount");

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        result.Items.Add(_repo.MapCaseLaw(row));
                    }
                }

                return ApiResponseFactory.Success<PagedResult<CaseLaw>>(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CaseLawService.Search");
                return ApiResponseFactory.Error<PagedResult<CaseLaw>>(Messages.ServerError);
            }
        }

        // ---------------- SAVE ----------------
        public ApiResponse<int> Save(SaveCaseLawRequest request)
        {
            try
            {
                ValidationResult v = ValidateSave(request);
                if (!v.IsValid)
                {
                    return ApiResponseFactory.ValidationError<int>(v.Errors);
                }

                int newId = _repo.ExecuteReturnNewId("SAVE", BuildSaveParams(request));
                return ApiResponseFactory.Success<int>(newId, "Case law saved successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CaseLawService.Save");
                return ApiResponseFactory.Error<int>(Messages.ServerError);
            }
        }

        // ---------------- UPDATE ----------------
        public ApiResponse<bool> Update(UpdateCaseLawRequest request)
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
                return ApiResponseFactory.Success<bool>(true, "Case law updated successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CaseLawService.Update");
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

                return ApiResponseFactory.Success<bool>(true, "Case law deleted successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CaseLawService.Delete");
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

        private ValidationResult ValidateSave(SaveCaseLawRequest r)
        {
            ValidationResult v = new ValidationResult();
            if (r == null)
            {
                v.Add("Request body is required.");
                return v;
            }

            // Adjust to your real mandatory fields.
            v.AddIfEmpty(r.FileName, "FileName");
            v.AddIfEmpty(r.IssueYear, "IssueYear");

            if (r.FileName != null && r.FileName.Length > 255)
                v.Add("FileName must not exceed 255 characters.");

            return v;
        }

        private SqlParameter[] BuildSaveParams(SaveCaseLawRequest r)
        {
            return new SqlParameter[]
            {
                SqlHelper.InParam("@FileName",        SqlDbType.NVarChar, r.FileName),
                SqlHelper.InParam("@Sections",        SqlDbType.NVarChar, r.Sections),
                SqlHelper.InParam("@Versus",          SqlDbType.NVarChar, r.Versus),
                SqlHelper.InParam("@Citation",        SqlDbType.NVarChar, r.Citation),
                SqlHelper.InParam("@Equi_Citation",   SqlDbType.NVarChar, r.Equi_Citation),
                SqlHelper.InParam("@CaseNo",          SqlDbType.NVarChar, r.CaseNo),
                SqlHelper.InParam("@Judge",           SqlDbType.NVarChar, r.Judge),
                SqlHelper.InParam("@Date_of_Judgement",SqlDbType.NVarChar, r.Date_of_Judgement),
                SqlHelper.InParam("@HeadNote",        SqlDbType.NVarChar, r.HeadNote),
                SqlHelper.InParam("@AdvocateName",    SqlDbType.NVarChar, r.AdvocateName),
                SqlHelper.InParam("@CasesReffered",   SqlDbType.NVarChar, r.CasesReffered),
                SqlHelper.InParam("@Category",        SqlDbType.NVarChar, r.Category),
                SqlHelper.InParam("@Type",            SqlDbType.NVarChar, r.Type),
                SqlHelper.InParam("@Subject",         SqlDbType.NVarChar, r.Subject),
                SqlHelper.InParam("@CitYear",         SqlDbType.NVarChar, r.CitYear),
                SqlHelper.InParam("@CourtName",       SqlDbType.NVarChar, r.CourtName),
                SqlHelper.InParam("@Vol",             SqlDbType.NVarChar, r.Vol),
                SqlHelper.InParam("@PageNO",          SqlDbType.NVarChar, r.PageNO),
                SqlHelper.InParam("@Books",           SqlDbType.NVarChar, r.Books),
                SqlHelper.InParam("@BusinessSupp",    SqlDbType.NVarChar, r.BusinessSupp),
                SqlHelper.InParam("@IssueMonth",      SqlDbType.NVarChar, r.IssueMonth),
                SqlHelper.InParam("@IssueYear",       SqlDbType.NVarChar, r.IssueYear),
                SqlHelper.InParam("@Judgement_date_new",  SqlDbType.DateTime, r.Judgement_date_new.HasValue ? (object)r.Judgement_date_new.Value : null),
                SqlHelper.InParam("@Equivalent_SCL",  SqlDbType.VarChar,  r.Equivalent_SCL),
                SqlHelper.InParam("@Equivalent_Comp_Cas",SqlDbType.VarChar,  r.Equivalent_Comp_Cas),
                SqlHelper.InParam("@Equivalent_CLJ",  SqlDbType.VarChar,  r.Equivalent_CLJ),
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