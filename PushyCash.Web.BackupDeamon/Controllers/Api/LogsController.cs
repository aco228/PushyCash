using PushyCash.Web.BackupDeamon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace PushyCash.Web.BackupDeamon.Controllers.Api
{
	public class LogsController : ApiController
	{

		[HttpGet]
		[Route("api/logs")]
		public List<LogModel> Get()
		{
			return BackupWebApplication.Logs;
		}
		
		[HttpGet]
		[Route("api/logs/add")]
		public List<LogModel> Add(string text)
		{
			BackupWebApplication.Log(text);
			return BackupWebApplication.Logs;
		}
		
	}
}