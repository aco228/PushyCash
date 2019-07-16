using PushyCash.TrafficNetworks.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PushyCash
{
	public class TrafficNetworkManager
	{
		private List<string> _listOfTrafficNetworksSuitableForExternalStorage = new List<string>();

		public Dictionary<string, TrafficSourceBase> Networks { get; protected set; } = new Dictionary<string, TrafficSourceBase>();
		public TrafficSourceBase this[string name] { get => Networks.ContainsKey(name) ? Networks[name] : null; }
		public List<TrafficSourceBase> NetworksList { get => (from n in this.Networks select n.Value).ToList(); }
		public Action OnLoadedAction = null;

		public int Clicks
		{
			get
			{
				int result = 0;
				foreach (var n in this.NetworksList) result += n.Clicks;
				return result;
			}
		}
		public int Conversions
		{
			get
			{
				int result = 0;
				foreach (var n in this.NetworksList) result += n.Conversions;
				return result;
			}
		}
		public double Payout
		{
			get
			{
				double result = 0;
				foreach (var n in this.NetworksList) result += n.Payout;
				return result;
			}
		}
		public double Spent
		{
			get
			{
				double result = 0;
				foreach (var n in this.NetworksList) result += n.Spent;
				return result;
			}
		}
		
		public TrafficNetworkManager()
		{
			this.Networks.Add("zeropark", new TrafficNetworks.Zeropark.ZeroparkManager());
			//this.Networks.Add("popads", new TrafficNetworks.PopAds.PopAdsManager());
			//this.Networks.Add("exoclick", new TrafficNetworks.Exoclick.ExoclickManager());
			this.Networks.Add("propellerads", new TrafficNetworks.PropellerAds.PropellerAdsManager());
		}
		
		#region # Network && Campaigns update #
		
		// async update
		public void Update()
		{
			foreach (KeyValuePair<string, TrafficSourceBase> network in Networks)
			{
				List<TrafficCampaign> campaigns = campaigns = this.TryLoadFromExternalStorage(network.Value.Name);
				if (campaigns == null || campaigns.Count == 0)
					this._listOfTrafficNetworksSuitableForExternalStorage.Add(network.Value.Name);
				
				network.Value.OnCampaignsLoadedAction += UpdateCampaignFinishCallback;
				network.Value.UpdateCampaignsAsync(campaigns);
			}
		}
		
		private int _UpdateCampaignFinishCallback_calledTimes = 0;
		public void UpdateCampaignFinishCallback(string trafficNetworkName)
		{
			if (!this.Networks.ContainsKey(trafficNetworkName))
				return;

			// save copy to external storage
			if (this._listOfTrafficNetworksSuitableForExternalStorage.Contains(trafficNetworkName))
				this.SaveStorageTrafficData(trafficNetworkName);

			foreach (TrafficCampaign c in this.Networks[trafficNetworkName].CampaignsList)
				if (!this.Networks[trafficNetworkName].Campaigns.ContainsKey(c.Key))
				{
					TrafficCampaign camp = this.MapMobileOperatorsAndDevices(c);
					this.Networks[trafficNetworkName].Campaigns.Add(camp.Key, camp);
					this.Networks[trafficNetworkName].CampaignIDMap.Add(camp.ID, camp);
				}
			
			_UpdateCampaignFinishCallback_calledTimes++;
			if (_UpdateCampaignFinishCallback_calledTimes == Networks.Count)
			{
				// stop all active campaigns from last time
				foreach (var ts in this.Networks)
					foreach (var c in ts.Value.CampaignsList)
						if (c.Data.IsCampaignActive)
							c.Stop();

				PushyContext.OnLoadFinished("TrafficManager");
				this.OnLoadedAction?.Invoke();
			}
		}

		// SUMMARY: After update of campaigns, update all deive and mnos to be compatible with Afflow namespaces 
		public TrafficCampaign MapMobileOperatorsAndDevices(TrafficCampaign campaign)
		{
			campaign.MobileOperator = PushyContext.AfflowManager.MobileOperatorMap.GetName(campaign.Country, campaign.MobileOperator);
			if (campaign.Device.Equals("android")) campaign.Device = "Android";
			if (campaign.Device.Equals("ios")) campaign.Device = "iOS";
			return campaign;
		}

		#endregion

		#region # External Storage #

		private List<TrafficCampaign> TryLoadFromExternalStorage(string trafficName)
		{
			FileInfo info = PushyExternalStorage.GetFileInfo(@"traffic_networks\traffic_campaigns_" + trafficName);
			if (!info.Exists)
				return null;

			List<TrafficCampaignData> data = PushyExternalStorage.ReadFromBinaryFile<List<TrafficCampaignData>>(info.FullName);
			if (data == null || data.Count == 0)
				return null;

			List<TrafficCampaign> result = new List<TrafficCampaign>();
			foreach (var tcd in data)
				result.Add((TrafficCampaign)Activator.CreateInstance(Type.GetType(tcd.Namespace), tcd));

			return result;
		}

		public void SaveStorageTrafficData()
		{
			foreach (var tsc in this.Networks)
				this.SaveStorageTrafficData(tsc.Key);
		}

		public void SaveStorageTrafficData(string trafficNetworkName)
		{
			if (!this.Networks.ContainsKey(trafficNetworkName))
				return;

			var source = this.Networks[trafficNetworkName];
			List<TrafficCampaignData> data = new List<TrafficCampaignData>();
			foreach (var campaigns in source.CampaignsList)
				data.Add(campaigns.Data);
			PushyExternalStorage.WriteToBinaryFile<List<TrafficCampaignData>>(PushyExternalStorage.GetFileInfo(@"traffic_networks\traffic_campaigns_" + source.Name).FullName, data);
		}

		#endregion

	}
}
