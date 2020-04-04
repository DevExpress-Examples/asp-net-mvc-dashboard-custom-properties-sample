using System;
using System.IO;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;

namespace AspMvcDashboardCustomPropertiesSample {
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication {
        protected void Application_Start() {
            DashboardConfig.RegisterService(RouteTable.Routes);
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            ModelBinders.Binders.DefaultBinder = new DevExpress.Web.Mvc.DevExpressEditorsBinder();

            DevExpress.Web.ASPxWebControl.CallbackError += Application_Error;
        }
        protected void Session_Start() {
            foreach(string file in Directory.EnumerateFiles(DashboardConfig.DashboardFolder, "*.xml")) {
                SessionDashboardStorage.Instance.RegisterDashboard(Path.GetFileName(file), XDocument.Load(file));
            }
        }
        protected void Application_Error(object sender, EventArgs e) {
            Exception exception = System.Web.HttpContext.Current.Server.GetLastError();
            //TODO: Handle Exception
        }
    }
}