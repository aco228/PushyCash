using System.Web;
using System.Web.Mvc;

namespace PushyCash.Web.Portal
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}
	}
}
