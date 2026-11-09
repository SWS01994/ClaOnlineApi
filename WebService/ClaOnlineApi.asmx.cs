using System.Web.Services;
using System.Web.Script.Services;
using WebAPI.BLL;

namespace WebAPI.WebService
{
    /// <summary>
    /// Summary description for ClaOnlineApi
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]   // REQUIRED for JSON
    //[System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public partial class ClaOnlineApi : System.Web.Services.WebService
    {

        private readonly ArticleService _articleService;
        private readonly CaseLawService _caseLawService;
        private readonly CircularService _circularService;
        private readonly NotificationService _notificationService;
        private readonly QueryService _queryService;
        private readonly LegislationService _legislationService;
        private readonly AuthService _authService;

        public ClaOnlineApi()
        {
            _authService = new AuthService();
            _articleService = new ArticleService();
            _caseLawService = new CaseLawService();
            _circularService = new CircularService();
            _notificationService = new NotificationService();
            _queryService = new QueryService();
            _legislationService = new LegislationService();
        }
    }
}
