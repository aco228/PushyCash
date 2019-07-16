using System.Web;
using System.Web.Mvc;

namespace PushyCash.Web.BackupDeamon
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}
	}
}
