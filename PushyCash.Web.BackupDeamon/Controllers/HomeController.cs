﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PushyCash.Web.BackupDeamon.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return this.Content("OK");
		}
		
	}
}