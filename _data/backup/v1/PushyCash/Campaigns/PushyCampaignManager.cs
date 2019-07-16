using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Campaigns
{
	public class PushyCampaignManager
	{
		public List<PushyCampaign> ActiveCampaigns { get; private set; } = new List<PushyCampaign>();

		public static double DailyBudget { get => PushyGlobal.DailyBudget; }
		public static double CurrentCost { get => PushyGlobal.Costs; }

		public void AddCampaign(PushyCampaign camp)
		{
			this.ActiveCampaigns.Add(camp);
		}

		public void RemoveCampaign(PushyCampaign camp)
		{
			this.ActiveCampaigns.Remove(camp);
		}
		
		public void Update()
		{
			foreach (var camp in this.ActiveCampaigns)
				camp.Update();
		}

	}
}
