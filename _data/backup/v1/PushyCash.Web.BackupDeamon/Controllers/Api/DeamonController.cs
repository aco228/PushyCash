using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace PushyCash.Web.BackupDeamon.Controllers.Api
{
	public class DeamonController : ApiController
	{

		[HttpGet]
		[Route("api/deamon/lastcall")]
		public DateTime? LastDeamonCall()
		{
			return BackupWebApplication.LastDeamonCall;
		}


		[HttpGet]
		[Route("api/deamon/call")]
		public object Call()
		{
			BackupWebApplication.LastDeamonCall = DateTime.Now;
			return new { status = true };
		}

	}
}