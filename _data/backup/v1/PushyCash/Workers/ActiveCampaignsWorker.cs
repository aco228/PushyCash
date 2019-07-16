using PushyCash.Afflow.Models;
using PushyCash.Campaigns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Workers
{
	public class ActiveCampaignsWorker : WorkerBase
	{
		public ActiveCampaignsWorker() : base("campaigns", "active_campaigns", 5000)
		{
		}

		public override void OnLoop()
		{
			if (PushyContext.CampaignManager.ActiveCampaigns == null || PushyContext.CampaignManager.ActiveCampaigns.Count == 0)
				return;
			
			PushyContext.CampaignManager.Update();
		}
		
	}
}
