using AppfailReporting.Mvc;
using System.Web;
using System.Web.Mvc;

namespace TestComplete
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AppfailReportAttribute()); // This needs to be added, before registering HandleErrorAttribute
            filters.Add(new HandleErrorAttribute());
        }
    }
}