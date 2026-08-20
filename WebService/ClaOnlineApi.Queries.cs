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
        public ApiResponse<QueryDetailResponse> GetQuery(int id)
        {
            return _queryService.Get(id);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public ApiResponse<PagedResult<Query>> SearchQueries(QuerySearchRequest request)
        {
            return _queryService.Search(request);
        }

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public ApiResponse<int> SaveQuery(SaveQueryRequest request)
        //{
        //    return _queryService.Save(request);
        //}

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public ApiResponse<bool> UpdateQuery(UpdateQueryRequest request)
        //{
        //    return _queryService.Update(request);
        //}

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public ApiResponse<bool> DeleteQuery(int id)
        //{
        //    return _queryService.Delete(id);
        //}
    }
}