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
        public ApiResponse<LegislationDetailResponse> GetLegislation(int id)
        {
            return _legislationService.Get(id);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public ApiResponse<PagedResult<Legislation>> SearchLegislations(LegislationSearchRequest request)
        {
            return _legislationService.Search(request);
        }

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public ApiResponse<int> SaveLegislation(SaveLegislationRequest request)
        //{
        //    return _legislationService.Save(request);
        //}

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public ApiResponse<bool> UpdateLegislation(UpdateLegislationRequest request)
        //{
        //    return _legislationService.Update(request);
        //}

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public ApiResponse<bool> DeleteLegislation(int id)
        //{
        //    return _legislationService.Delete(id);
        //}
    }
}