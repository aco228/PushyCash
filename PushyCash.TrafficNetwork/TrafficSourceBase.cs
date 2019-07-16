using PushyCash.Direct.Models.TrafficNetworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PushyCash.TrafficNetworks.Core
{
	public enum TrafficSourceTimeZone { Belgrade, London }

	public abstract class TrafficSourceBase
	{
		public string Name { get; protected set; } = string.Empty;
		public int DirectModelID { get; protected set; } = -1;
		public double DailyBudget { get; protected set; } = 20.0;
		public TrafficNetworkApiBase API { get; protected set; } = null;
		public TrafficSourceAfflowMapBase AfflowMap { get; protected set; } = null;
		public List<TrafficCampaign> CampaignsList { get; protected set; } = new List<TrafficCampaign>();
		public Dictionary<string, TrafficCampaign> Campaigns { get; protected set; } = new Dictionary<string, TrafficCampaign>(); // MA.INWI.mainstream.Android
		public Dictionary<string, TrafficCampaign> CampaignIDMap { get; protected set; } = new Dictionary<string, TrafficCampaign>(); // id of the campaign
		public Action<string> OnCampaignsLoadedAction = null;
		public TrafficCampaign this[string key] { get => Campaigns.ContainsKey(key) ? Campaigns[key] : null; }

		public virtual int OptimizationTemporaryLimit { get; } = 60;
		public virtual int OptimizationStopageLimit { get; } = 150;
		public virtual bool SupportsAutoBid { get; } = true;
		public virtual bool HasConversionsReport { get; } = true;
		public virtual bool HasPayoutReport { get; } = true;
		public virtual bool SupportsOptimization { get; } = true;
		public virtual TrafficSourceTimeZone Timezone { get; } = TrafficSourceTimeZone.Belgrade;

		public TrafficSourceTodayStats TodayStats { get; protected set; } = null;
		//public int Clicks { get => this.TodayStats == null ? 0 : this.TodayStats.Redirects; }
		//public int Conversions { get => this.TodayStats == null ? 0 : this.TodayStats.Conversions; }
		//public double Spent { get => this.TodayStats == null ? 0 : this.TodayStats.Spent; }
		//public double Payout { get => this.TodayStats == null ? 0 : this.TodayStats.Payout; }

		public int Clicks { get; set; } = 0;
		public int Conversions { get; set; } = 0;
		public double Payout { get; set; } = 0.0;
		public double Spent { get; set; } = 0.0;
		
		public TrafficSourceBase(string name, TrafficSourceAfflowMapBase afflowMap, TrafficNetworkApiBase api)
		{
			this.Name = name;
			this.AfflowMap = afflowMap;
			this.API = api;
			this.API.Manager = this;

			DirectTrafficNetwork direct =  DirectTrafficNetwork.Load(this.Name, this.AfflowMap.NID);
			this.DirectModelID = direct.TrafficNetworkID;
			this.DailyBudget = direct.DailyBudget;
		}

		// SUMMARY: Update campaigns 
		public List<TrafficCampaign> LoadAndUpdateCampaigns()
		{
			this.CampaignsList = this.LoadCampaignList();
			foreach (TrafficCampaign tc in this.CampaignsList)
			{
				//if (!this.Campaigns.ContainsKey(tc.ID))
				//	this.Campaigns.Add(tc.ID, tc);
				tc.MapWithTrafficSource(this);
			}

			return this.CampaignsList;
		}
		public void UpdateCampaignsAsync(List<TrafficCampaign> preloadedCampaigns)
		{
			new Thread(() =>
			{
				if (preloadedCampaigns == null || preloadedCampaigns.Count == 0)
					this.CampaignsList = this.LoadCampaignList();
				else
					this.CampaignsList = preloadedCampaigns;

				foreach (TrafficCampaign tc in this.CampaignsList)
				{
					//if(!this.Campaigns.ContainsKey(tc.ID))
					//	this.Campaigns.Add(tc.ID, tc);
					tc.MapWithTrafficSource(this);
				}
								
				this.OnCampaignsLoadedAction?.Invoke(this.Name);
			}).Start();
		}
		public virtual List<TrafficCampaign> LoadCampaignList() { return this.API.GetCampaignsList(); }
		public virtual IEnumerable<TrafficCampaign> LoadCampaigns() { return this.API.GetCampaigns(); }

		// obsolete
		public TrafficSourceTodayStats GetTodayStats() { this.TodayStats = this.GetTodayStatsImplementation(); return this.TodayStats; }
		protected virtual TrafficSourceTodayStats GetTodayStatsImplementation() { return this.API.GetTodayStats(); }

		public IEnumerable<TrafficSourceTodayStats> GetCampaignTodayStats() { return this.GetCampaignTodayStatsImplementation();  }
		protected virtual IEnumerable<TrafficSourceTodayStats> GetCampaignTodayStatsImplementation() { return this.API.GetCampaignTodayStats(); }

		public abstract TrafficCampaignData ConvertJson(dynamic json);

	}
}
