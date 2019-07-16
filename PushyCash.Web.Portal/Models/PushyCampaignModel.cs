using PushyCash.Web.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PushyCash.Web.Portal.Models
{
	public class PushyCampaignModel
	{
		public PushyCampaignWebModel Campaign = null;
    public Dictionary<string, PushyNetworkCampaignModel> NetworkCampaigns = new Dictionary<string, PushyNetworkCampaignModel>();

	}
}