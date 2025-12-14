using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Admin
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error()
        {
            var ex = Server.GetLastError() as HttpException;

            if (ex != null && ex.Message.Contains("Maximum request length"))
            {
                Server.ClearError();

                HttpContext.Current.Session["UploadError"] =
                    "Ảnh tải lên quá dung lượng cho phép (tối đa 2MB). Vui lòng chọn ảnh nhỏ hơn.";

                HttpContext.Current.Response.Redirect("~/Account/Profile");
            }
        }
    }
}
