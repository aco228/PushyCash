using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace PushyCash.Web.BackupDeamon.Controllers.Api
{
	public class CampaignsController : ApiController
	{

		[HttpGet]
		[Route("api/campaigns/start")]
		public object Start(string source, string id)
		{
			if (!PushyContext.TrafficNetworkManager.Networks.ContainsKey(source))
				return new { status = false, message = "THere is not network with name = " + source };

			bool result = PushyContext.TrafficNetworkManager.Networks[source].API.StartCampaign(id);
			return new { status = result, campaign = id };
		}

		[HttpGet]
		[Route("api/campaigns/stop")]
		public object Stop(string source, string id)
		{
			if (!PushyContext.TrafficNetworkManager.Networks.ContainsKey(source))
				return new { status = false, message = "THere is not network with name = " + source };

			bool result = PushyContext.TrafficNetworkManager.Networks[source].API.StopCampaign(id);
			return new { status = result, campaign = id };
		}

	}
}