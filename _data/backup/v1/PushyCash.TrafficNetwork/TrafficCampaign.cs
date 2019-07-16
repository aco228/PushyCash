using PushyCash.Direct;
using PushyCash.Direct.Models.TrafficNetworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.TrafficNetworks.Core
{
	public enum TrafficCampaignStatus { Active, Paused, None }
	public enum TrafficOptimizationResult { None, RemovedTemporary, RemovedPermanently, RemovedTemporaryAndPermanently }
	
	public abstract class TrafficCampaign
	{
		public TrafficSourceBase TrafficSource { get; protected set; } = null;
		public TrafficNetworkApiBase API { get => this.TrafficSource != null ? this.TrafficSource.API : null; }
		public TrafficSourceAfflowMapBase AfflowMap { get => this.TrafficSource != null ? this.TrafficSource.AfflowMap : null; }
		public int? DirectModelID
		{
			get
			{
				if (this.Data.DirectModelID.HasValue && this.Data.DirectModelID.Value != -1)
					return this.Data.DirectModelID.Value;

				this.Data.DirectModelID = PushyCashDirect.Instance.LoadInt("SELECT TrafficNetworkCampaignID FROM [].TrafficNetworkCampaign WHERE CampaignID={0} AND TrafficNetworkID={1}", this.ID, this.TrafficSource.DirectModelID);
				return this.Data.DirectModelID.HasValue ? this.DirectModelID.Value : -1;
			}
		}
		public TrafficCampaignData Data { get; set; } = null;
		public string Key { get => string.Format("{0}.{1}.{2}.{3}", this.Country, this.MobileOperator, this.Vertical, this.Device); }
		public List<string> AlreadyStopedTargets { get; protected set; } = null;
		public List<TrafficTargetsInfo> TemporaryStopedTargets { get; protected set; } = new List<TrafficTargetsInfo>();

		public TrafficCurrentStats InitialStats { get; set; } = new TrafficCurrentStats();
		public TrafficCurrentStats CurrentStats { get; set; } = null;
		public Direct.Models.DirectPushyNetworkCampaign LastDirectPushyNetworkCampaign { get; set; } = null;

		public int Clicks => this.InitialStats.Clicks + (this.CurrentStats != null ? this.CurrentStats.Clicks : 0);
		public int Conversions => this.InitialStats.Conversions + (this.CurrentStats != null ? this.CurrentStats.Conversions : 0);
		public double Payout => this.InitialStats.Payout + (this.CurrentStats != null ? this.CurrentStats.Payout : 0);
		public double Spent => this.InitialStats.Spent + (this.CurrentStats != null ? this.CurrentStats.Spent : 0);

		public string ID { get => this.Data.ID; }
		public string TrafficNetworkName { get => this.Data.TrafficNetworkName; }
		public string FullName { get => this.Data.FullName; }
		public string Country { get => this.Data.Country; }
		public string Vertical { get => this.Data.Vertical; }
		public string MobileOperator { get => this.Data.MobileOperator; set => this.Data.MobileOperator = value; }
		public string Device { get => this.Data.Device; set => this.Data.Device = value; }
		public Afflow.Models.AfflowLink Afflowlink { get; set; } = null;
		public TrafficCampaignStatus Status { get; set; } = TrafficCampaignStatus.None;
		public double BidValue { get; protected set; } = 0.0001;
		public int LastVisibility = 1;

		public TrafficCampaign(TrafficCampaignData data)
		{
			this.Data = data;
		}
		public void MapWithTrafficSource(TrafficSourceBase source)
		{
			this.TrafficSource = source;
			if (!this.Data.DirectModelID.HasValue)
				this.Data.DirectModelID = DirectTrafficNetworkCampaign.Load(this.TrafficSource.DirectModelID, this.FullName, this.ID);
		}
		public virtual TrafficCampaignData Reload()
		{
			int? directModelID = this.DirectModelID;
			var data = this.API.ReloadCampaign(this);
			if (this.Data == null)
				return this.Data;

			this.Data = data;
			this.Data.DirectModelID = directModelID;

			if (this.Data.Visibility != 0)
				this.LastVisibility = this.Data.Visibility;
			return this.Data;
		}
		public virtual TrafficCurrentStats GetCurrentStats() { return this.API.GetCurrentStats(this);  }
		public virtual void Start() { this.API.StartCampaign(this); }
		public virtual void Stop()
		{
			for(; ;)
			{
				if (this.API.StopCampaign(this))
					break;
				System.Threading.Thread.Sleep(300);
			}
			this.Data.IsCampaignActive = false;
		}
		public virtual bool AutoBid()
		{
			return false;
		}
		public virtual bool Bid(double bid)
		{
			if (this.BidValue == bid)
				return false;

			this.BidValue = bid;
			return this.API.Bid(this);
		}
		public virtual TrafficOptimizationResult TryOptimization(List<TrafficTargetsInfo> targetInfo, bool initialRun)
		{
			if (this.AlreadyStopedTargets == null)
				this.AlreadyStopedTargets = DirectTrafficNetworkCampaign.LoadBlacklist(this.DirectModelID.Value);

			TrafficOptimizationResult result = TrafficOptimizationResult.None;
			List<TrafficTargetsInfo> targetsForTemporary = new List<TrafficTargetsInfo>();
			List<TrafficTargetsInfo> targetsForStopage = new List<TrafficTargetsInfo>();
			foreach(var ti in targetInfo)
			{
				if (ti.Conversions != 0)
					continue;

				if (this.AlreadyStopedTargets.Contains(ti.Name))
					continue;
				if(ti.Clicks > 100 && ti.BotRisk > 55)
					targetsForStopage.Add(ti);
				else if (ti.Clicks >= this.TrafficSource.OptimizationStopageLimit)
					targetsForStopage.Add(ti);
				else if ( !initialRun && ti.Clicks >= this.TrafficSource.OptimizationTemporaryLimit)
					targetsForTemporary.Add(ti);
			}

			if(targetsForStopage.Count > 0)
			{
				DirectTrafficNetworkCampaign.AddNewBlacklistValues(this.DirectModelID.Value, targetsForStopage.ToStringList());
				this.AlreadyStopedTargets.AddRange(targetsForStopage.ToStringList());
				result = TrafficOptimizationResult.RemovedPermanently;
			}

			if(targetsForTemporary.Count > 0)
			{
				if (result == TrafficOptimizationResult.RemovedPermanently)
					result = TrafficOptimizationResult.RemovedTemporaryAndPermanently;
				else
					result = TrafficOptimizationResult.RemovedPermanently;
			}

			this.TemporaryStopedTargets.AddRange(TemporaryStopedTargets);
			targetsForStopage.AddRange(TemporaryStopedTargets);
			this.API.StopTargets(this, targetsForStopage);

			return result;
		}
		public virtual void ResumeTemporaryStopedTargets()
		{
			this.API.ResumeTargets(this, this.TemporaryStopedTargets);
			this.TemporaryStopedTargets.Clear();
		}
	}
}
