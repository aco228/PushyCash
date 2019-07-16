using Direct.Core;
using PushyCash.Direct;
using PushyCash.Direct.Models.PushyCampaign;
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
		private DirectTrafficCampaignDayStat _stats = null;

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
		public DirectTrafficCampaignDayStat Stats
		{
			get
			{
				if(this._stats == null)
				{
					this._stats =
						PushyCashDirect.Instance.LoadContainer(
							@"SELECT * FROM [].TrafficCampaignDayStat WHERE TrafficNetworkCampaignID={0} AND TrafficNetworkID={1} AND Day={2}",
							this.DirectModelID.Value, this.TrafficSource.DirectModelID, DirectHelper.GetDateTime(DateTime.Today)).Convert<DirectTrafficCampaignDayStat>();

					if(this._stats == null || this._stats.ID() == -1)
					{
						this._stats = new DirectTrafficCampaignDayStat()
						{
							TrafficNetworkCampaignID = this.DirectModelID.Value,
							TrafficNetworkID = this.TrafficSource.DirectModelID,
							Day = DateTime.Today
						};
						PushyCashDirect.Instance.Insert(this._stats);
					}
				}

				if(DateTime.Today != this._stats.Day)
				{
					this._stats = null;
					return this.Stats;
				}

				return this._stats;
			}
		}

		public bool AutoRunning { get => this.Stats.AutoRunning; }
		public bool IsRunning { get => this.Stats.IsRunning; }
		public DateTime? LastConversion { get; protected set; } = null;
		public DateTime? Started { get => this.Stats.Started; }
		public DateTime? Ended { get => this.Stats.Ended; }

		public int Clicks => this.Stats.Clicks;
		public int Conversions => this.Stats.Conversions;
		public double Payout => this.Stats.Payout;
		public double Spent => this.Stats.Costs;

		public string ID { get => this.Data.ID; }
		public string TrafficNetworkName { get => this.Data.TrafficNetworkName; }
		public string FullName { get => this.Data.FullName; }
		public string Country { get => this.Data.Country; }
		public string Vertical { get => this.Data.Vertical; }
		public string MobileOperator { get => this.Data.MobileOperator; set => this.Data.MobileOperator = value; }
		public string Device { get => this.Data.Device; set => this.Data.Device = value; }
		public Afflow.Models.AfflowLink Afflowlink { get; set; } = null;
		public TrafficCampaignStatus Status { get; set; } = TrafficCampaignStatus.None;
		public double? BidValue { get; protected set; } = null;
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
			this.PostMapingTrafficSourceFunctionality();
		}
		public virtual void PostMapingTrafficSourceFunctionality()
		{

		}
		public virtual TrafficCampaignData Reload()
		{
			int? directModelID = this.DirectModelID;
			var data = this.API.ReloadCampaign(this);
			if (data == null)
				return this.Data;

			this.Data = data;
			this.Data.DirectModelID = directModelID;

			if (this.Data.Visibility != 0)
				this.LastVisibility = this.Data.Visibility;
			return this.Data;
		}
		public virtual TrafficCurrentStats GetCurrentStats() { return this.API.GetCurrentStats(this);  }
		public virtual void Start()
		{
			this.Stats.Ended = null;
			this.Stats.Started = DateTime.Now;
			this.Stats.IsRunning = true;
			this.Stats.AutoRunning = false;
			this.Stats.Update();

			DirectActiveCampaign.Insert(this.ID, this.TrafficNetworkName);
			this.API.StartCampaign(this);
		}
		public virtual void Stop()
		{
			for(; ;)
			{
				if (this.API.StopCampaign(this))
				{
					this.Stats.Ended = DateTime.Now;
					this.Stats.Started = null;
					this.Stats.IsRunning = false;
					this.Stats.AutoRunning = false;
					this.Stats.Update();

					this.ResumeTemporaryStopedTargets();
					DirectActiveCampaign.Delete(this.ID, this.TrafficNetworkName);
					break;
				}
				System.Threading.Thread.Sleep(300);
			}
			this.Data.IsCampaignActive = false;
		}
		public virtual void OnConversion()
		{
			LastConversion = DateTime.Now;
			if(!this.IsRunning && this.Ended.HasValue && (DateTime.Now - this.Ended.Value).TotalMinutes > 45 && this.Stats.ROI >= 1.5)
			{
				this.Stats.Log($"Starting again campaign because there were conversions and ROI is above 1.5...");
				this.Start();

				this.Stats.AutoRunning = true;
				this.Stats.Update();
			}
		}
		public virtual void CheckAutoRunner()
		{
			if (!this.AutoRunning)
				return;

			if (this.Stats.ROI < 0)
			{
				this.Stats.Log("Stoping AutoRunner because ROI went bellow 0");
				this.Stop();
			}
		}
		public virtual void BeforeAutoBiddingStarts() { }
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
		public virtual TrafficOptimizationResult RunOptimization(List<TrafficTargetsInfo> targetInfo, bool initialRun)
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

				if (this.AlreadyStopedTargets.Contains(ti.Name) || 
					(from t in this.TemporaryStopedTargets where t.Name.Equals(ti.Name) select t).FirstOrDefault() != null)
					continue;
				
				if(ti.Clicks > 50 && ti.BotRisk > 55)
				{
					ti.Reason = $"Click={ti.Clicks} and BotRist={ti.BotRisk}";
					DirectTrafficNetworkCampaign.AddBlackListValue(this.DirectModelID.Value, ti.Name, ti.Reason);
					targetsForStopage.Add(ti);
				}

				if (ti.Clicks >= this.TrafficSource.OptimizationStopageLimit)
				{
					ti.Reason = $"Clicks={ti.Clicks} >= OptimizationStopageLimit={this.TrafficSource.OptimizationStopageLimit}";
					DirectTrafficNetworkCampaign.AddBlackListValue(this.DirectModelID.Value, ti.Name, ti.Reason);
					targetsForStopage.Add(ti);
				}
				else if (!initialRun && ti.CorrectedClicks >= this.TrafficSource.OptimizationTemporaryLimit)
				{
					ti.Reason = $"Clicks={ti.Clicks} >= OptimizationTemporaryLimit={this.TrafficSource.OptimizationTemporaryLimit}";
					DirectTrafficNetworkCampaign.AddTemporaryBlackListValue(this.DirectModelID.Value, ti.Name, ti.Reason);
					targetsForTemporary.Add(ti);
				}

			}

			if(targetsForStopage.Count > 0)
			{
				this.AlreadyStopedTargets.AddRange(targetsForStopage.ToStringList());
				this.Stats.Log("Some target are permanently stoped");
				result = TrafficOptimizationResult.RemovedPermanently;
			}

			if(targetsForTemporary.Count > 0)
			{
				this.Stats.Log("Some target are temporary stoped");
				if (result == TrafficOptimizationResult.RemovedPermanently)
					result = TrafficOptimizationResult.RemovedTemporaryAndPermanently;
				else
					result = TrafficOptimizationResult.RemovedTemporary;
			}

			this.TemporaryStopedTargets.AddRange(TemporaryStopedTargets);
			targetsForStopage.AddRange(TemporaryStopedTargets);
			this.API.StopTargets(this, targetsForStopage);

			return result;
		}
		public virtual void ResumeTemporaryStopedTargets()
		{
			this.API.ResumeTargets(this, this.TemporaryStopedTargets);
			foreach(var t in this.TemporaryStopedTargets)
				DirectTrafficNetworkCampaign.RemoveTemporaryBlackListValue(this.DirectModelID.Value, t.Name);

			this.TemporaryStopedTargets.Clear();
		}
		public virtual TrafficCampaignStartMetrics CheckIfThereIsRisk()
		{
			// TODO: Implement this shit
			return new TrafficCampaignStartMetrics();
		}
	}
}
