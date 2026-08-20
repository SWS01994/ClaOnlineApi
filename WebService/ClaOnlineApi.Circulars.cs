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
        public ApiResponse<CircularDetailResponse> GetCircular(int id)
        {
            return _circularService.Get(id);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public ApiResponse<PagedResult<Circular>> SearchCirculars(CircularSearchRequest request)
        {
            return _circularService.Search(request);
        }

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public ApiResponse<int> SaveCircular(SaveCircularRequest request)
        //{
        //    return _circularService.Save(request);
        //}

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public ApiResponse<bool> UpdateCircular(UpdateCircularRequest request)
        //{
        //    return _circularService.Update(request);
        //}

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public ApiResponse<bool> DeleteCircular(int id)
        //{
        //    return _circularService.Delete(id);
        //}
    }
}