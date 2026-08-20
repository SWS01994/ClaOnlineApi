using System.Collections.Generic;
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
        public ApiResponse<CaseLawDetailResponse> GetCaseLaw(int id)
        {
            return _caseLawService.Get(id);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public ApiResponse<PagedResult<CaseLaw>> SearchCaseLaws(CaseLawSearchRequest request)
        {
            return _caseLawService.Search(request);
        }

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public ApiResponse<int> SaveCaseLaw(SaveCaseLawRequest request)
        //{
        //    return _caseLawService.Save(request);
        //}

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public ApiResponse<bool> UpdateCaseLaw(UpdateCaseLawRequest request)
        //{
        //    return _caseLawService.Update(request);
        //}

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public ApiResponse<bool> DeleteCaseLaw(int id)
        //{
        //    return _caseLawService.Delete(id);
        //}
    }
}