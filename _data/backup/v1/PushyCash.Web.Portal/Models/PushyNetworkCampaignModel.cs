using PushyCash.Web.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PushyCash.Web.Portal.Models
{
	public class PushyNetworkCampaignModel
	{
		public PushyNetworkCampaignWebModel Campaign = null;
		public List<PushyNetworkCampaignLog> Logs = new List<PushyNetworkCampaignLog>();
	}
}