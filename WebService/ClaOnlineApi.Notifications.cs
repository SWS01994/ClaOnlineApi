using System.Web.Script.Services;
using System.Web.Services;
using WebAPI.BLL;
using WebAPI.Models.Entities;
using WebAPI.Models.Requests;
using WebAPI.Models.Responses;

namespace WebAPI.WebService
{
    public partial class ClaOnlineApi
    {
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public ApiResponse<NotificationDetailResponse> GetNotification(int id)
        {
            return _notificationService.Get(id);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public ApiResponse<PagedResult<Notification>> SearchNotifications(NotificationSearchRequest request)
        {
            return _notificationService.Search(request);
        }

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public ApiResponse<int> SaveNotification(SaveNotificationRequest request)
        //{
        //    return _notificationService.Save(request);
        //}

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public ApiResponse<bool> UpdateNotification(UpdateNotificationRequest request)
        //{
        //    return _notificationService.Update(request);
        //}

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public ApiResponse<bool> DeleteNotification(int id)
        //{
        //    return _notificationService.Delete(id);
        //}
    }
}