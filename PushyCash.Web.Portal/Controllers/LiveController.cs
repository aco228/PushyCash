using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PushyCash.Web.Portal.Controllers
{
	public class LiveController : Controller
	{

		public ActionResult Index()
		{
			return this.View();
		}

	}
}