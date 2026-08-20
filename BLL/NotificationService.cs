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
    public class NotificationService
    {
        private readonly NotificationRepository _repo;

        public NotificationService()
        {
            _repo = new NotificationRepository();
        }

        // ---------------- GET ONE ----------------
        public ApiResponse<NotificationDetailResponse> Get(int id)
        {
            try
            {
                if (id <= 0)
                {
                    List<string> errs = new List<string>();
                    errs.Add("A valid id is required.");
                    return ApiResponseFactory.ValidationError<NotificationDetailResponse>(errs);
                }

                DataSet ds = _repo.Execute("GETBYID", new SqlParameter[]
                {
                    SqlHelper.InParam("@Id", SqlDbType.Int, id)
                });

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    return ApiResponseFactory.NotFound<NotificationDetailResponse>(Messages.NotFound);
                }

                NotificationDetailResponse payload = new NotificationDetailResponse();
                payload.Notification = _repo.MapNotification(ds.Tables[0].Rows[0]);

                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    payload.Content = _repo.MapNotificationData(ds.Tables[1].Rows[0]);
                }

                return ApiResponseFactory.Success<NotificationDetailResponse>(payload);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "NotificationService.Get");
                return ApiResponseFactory.Error<NotificationDetailResponse>(Messages.ServerError);
            }
        }

        // ---------------- SEARCH ----------------
        public ApiResponse<PagedResult<Notification>> Search(NotificationSearchRequest request)
        {
            try
            {
                if (request == null) request = new NotificationSearchRequest();
                request.Normalize();

                if (request.FromDate.HasValue && request.ToDate.HasValue
                    && request.FromDate.Value > request.ToDate.Value)
                {
                    List<string> errs = new List<string>();
                    errs.Add("FromDate cannot be later than ToDate.");
                    return ApiResponseFactory.ValidationError<PagedResult<Notification>>(errs);
                }

                DataSet ds = _repo.Execute("SEARCH", new SqlParameter[]
                {
                    SqlHelper.InParam("@FromDate",   SqlDbType.Date,     request.FromDate.HasValue ? (object)request.FromDate.Value : null),
                    SqlHelper.InParam("@ToDate",     SqlDbType.Date,     request.ToDate.HasValue ? (object)request.ToDate.Value : null),
                    SqlHelper.InParam("@PageNumber", SqlDbType.Int,      request.PageNumber),
                    SqlHelper.InParam("@PageSize",   SqlDbType.Int,      request.PageSize)
                });

                PagedResult<Notification> result = new PagedResult<Notification>();
                result.Items = new List<Notification>();
                result.PageNumber = request.PageNumber;
                result.PageSize = request.PageSize;
                result.TotalCount = 0;

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    result.TotalCount = ds.Tables[0].Rows[0].GetInt("TotalCount");

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        result.Items.Add(_repo.MapNotification(row));
                    }
                }

                return ApiResponseFactory.Success<PagedResult<Notification>>(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "NotificationService.Search");
                return ApiResponseFactory.Error<PagedResult<Notification>>(Messages.ServerError);
            }
        }

        // ---------------- SAVE ----------------
        public ApiResponse<int> Save(SaveNotificationRequest request)
        {
            try
            {
                ValidationResult v = ValidateSave(request);
                if (!v.IsValid)
                {
                    return ApiResponseFactory.ValidationError<int>(v.Errors);
                }

                int newId = _repo.ExecuteReturnNewId("SAVE", BuildSaveParams(request));
                return ApiResponseFactory.Success<int>(newId, "Notification saved successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "NotificationService.Save");
                return ApiResponseFactory.Error<int>(Messages.ServerError);
            }
        }

        // ---------------- UPDATE ----------------
        public ApiResponse<bool> Update(UpdateNotificationRequest request)
        {
            try
            {
                if (request == null || request.Id <= 0)
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

                if (!Exists(request.Id))
                {
                    return ApiResponseFactory.NotFound<bool>(Messages.NotFound);
                }

                SqlParameter[] p = BuildSaveParams(request);
                SqlParameter[] withId = Prepend(p, SqlHelper.InParam("@Id", SqlDbType.Int, request.Id));

                _repo.ExecuteNonQuery("UPDATE", withId);
                return ApiResponseFactory.Success<bool>(true, "Notification updated successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "NotificationService.Update");
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
                    SqlHelper.InParam("@Id", SqlDbType.Int, id)
                });

                return ApiResponseFactory.Success<bool>(true, "Notification deleted successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "NotificationService.Delete");
                return ApiResponseFactory.Error<bool>(Messages.ServerError);
            }
        }

        // ================= private helpers =================

        private bool Exists(int id)
        {
            DataSet ds = _repo.Execute("GETBYID", new SqlParameter[]
            {
                SqlHelper.InParam("@Id", SqlDbType.Int, id)
            });
            return ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0;
        }

        private ValidationResult ValidateSave(SaveNotificationRequest r)
        {
            ValidationResult v = new ValidationResult();
            if (r == null)
            {
                v.Add("Request body is required.");
                return v;
            }

            // Adjust to your real mandatory fields.
            v.AddIfEmpty(r.NotificationNo, "NotificationNo");
            v.AddIfEmpty(r.IssueYear, "IssueYear");

            return v;
        }

        private SqlParameter[] BuildSaveParams(SaveNotificationRequest r)
        {
            return new SqlParameter[]
            {
                SqlHelper.InParam("@Title",               SqlDbType.NVarChar, r.Title),
                SqlHelper.InParam("@FileNo",              SqlDbType.NVarChar, r.FileNo),
                SqlHelper.InParam("@NotificationDate",    SqlDbType.NVarChar, r.NotificationDate),
                SqlHelper.InParam("@Type",                SqlDbType.NVarChar, r.Type),
                SqlHelper.InParam("@NotificationNo",      SqlDbType.NVarChar, r.NotificationNo),
                SqlHelper.InParam("@Summary",             SqlDbType.NVarChar, r.Summary),
                SqlHelper.InParam("@Category",            SqlDbType.NVarChar, r.Category),
                SqlHelper.InParam("@Subject",             SqlDbType.NVarChar, r.Subject),
                SqlHelper.InParam("@Books",               SqlDbType.NVarChar, r.Books),
                SqlHelper.InParam("@Vol",                 SqlDbType.NVarChar, r.Vol),
                SqlHelper.InParam("@Sections",            SqlDbType.NVarChar, r.Sections),
                SqlHelper.InParam("@IssueMonth",          SqlDbType.NVarChar, r.IssueMonth),
                SqlHelper.InParam("@IssueYear",           SqlDbType.NVarChar, r.IssueYear),
                SqlHelper.InParam("@FileName",            SqlDbType.NVarChar, r.FileName),
                SqlHelper.InParam("@Citation",            SqlDbType.NVarChar, r.Citation),
                SqlHelper.InParam("@Equi_Citation",       SqlDbType.NVarChar, r.Equi_Citation),
                SqlHelper.InParam("@Equivalent_SCL",      SqlDbType.NVarChar, r.Equivalent_SCL),
                SqlHelper.InParam("@Equivalent_Comp_Cas",    SqlDbType.NVarChar, r.Equivalent_Comp_Cas),
                SqlHelper.InParam("@Equivalent_CLJ",      SqlDbType.NVarChar, r.Equivalent_CLJ),
                SqlHelper.InParam("@NotificationDate_new",SqlDbType.DateTime, r.NotificationDate_new.HasValue ? (object)r.NotificationDate_new.Value : null),
                SqlHelper.InParam("@ShowOnHome",          SqlDbType.Bit,      r.ShowOnHome),
                SqlHelper.InParam("@Filetext",            SqlDbType.NVarChar, r.Filetext),
                SqlHelper.InParam("@FileHTML",            SqlDbType.NVarChar, r.FileHTML),
                SqlHelper.InParam("@Srn",                 SqlDbType.Int,      r.Srn)
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