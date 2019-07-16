using Direct.Core;
using PushyCash.Afflow.Models.Stats;
using PushyCash.Direct;
using PushyCash.Direct.Models;
using PushyCash.Direct.Models.PushyCampaign;
using PushyCash.TrafficNetworks.Core;
using PushyCash.Web.Core.Models;
using PushyCash.Workers.SingleThreadWorkers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PushyCash.Campaigns
{
	public class PushyNetworkCampaign
	{
		public PushyCampaign PushyCampaign { get; protected set; } = null;
		public DateTime? LastOptimizationChecked { get; protected set; } = null;
		public List<TrafficTargetsInfo> InitialTrafficTargetReport { get; protected set; } = null;
		public AfflowInitialCampaignData InitialAfflowInformations { get; protected set; } = null;
		public List<PushyNetworkCampaignLog> Logs { get; protected set; } = new List<PushyNetworkCampaignLog>();
		public PushyNetworkCampaignWebModel WebModel { get; set; } = null;
		public PushyNetworkCampaignWebModelStatusChanger StatusEventChange { get; set; } = null;

		public TrafficCampaign TrafficCampaign { get; protected set; } = null;
		public DirectPushyNetworkCampaign DirectPushyCampaign { get; protected set; } = null;
		public string CampaignID { get => TrafficCampaign.ID; }
		public string NID { get => TrafficCampaign.Afflowlink.nid; }
		public bool IsStoped { get; protected set; } = false;
		public bool IsPaused { get => this.PausedTimeout.HasValue; }
		public DateTime? PausedTimeout { get; protected set; } = null;
		public DateTime Created { get; protected set; } = DateTime.Now;

		public int ClickskPerConversion { get => this.PushyCampaign.Metrics.LinkRecomendedClicksWithoutConversion; }
		public double Budget { get; private set; } = 0.0;
		public double ToleratedDeficit { get => this.PushyCampaign.AllowedDeficit; }
		
		public Models.CampaignInfoStat InitialAfflowStats = new Models.CampaignInfoStat(); // initial 
		public Models.CampaignInfoStat InitialTrafficStats = new Models.CampaignInfoStat();

		public int Clicks { get => this.TrafficCampaign.CurrentStats.Clicks; }
		public int Conversions { get => this.TrafficCampaign.CurrentStats.Conversions; }
		public double Bid { get => TrafficCampaign.BidValue; }
		public double Payout { get => this.TrafficCampaign.CurrentStats.Payout; }
		public double Spent { get => this.TrafficCampaign.CurrentStats.Spent; }
		public double EarnedMoney { get => this.Payout - this.Spent; }
		public double ROI { get => this.Spent == 0 || this.Payout == 0 ? -100.0 : ((this.Payout - this.Spent) / this.Spent) * 100; }
		
		public PushyNetworkCampaign(PushyCampaign pushyCampaign, TrafficCampaign campaign)
		{
			PLogger.Debug($"Link '{pushyCampaign.Link.FullName}' is starting network '{campaign.TrafficNetworkName}'");
			this.PushyCampaign = pushyCampaign;
			this.Budget = this.PushyCampaign.Metrics.LinkRecomendedBudget;

			// prepare traffic campaign for tracking
			this.TrafficCampaign = campaign;
			this.TrafficCampaign.LastDirectPushyNetworkCampaign = null;
			this.TrafficCampaign.CurrentStats = new TrafficCurrentStats();

			this.LoadInitialCampaignStats();

			this.DirectPushyCampaign = new DirectPushyNetworkCampaign();
			this.DirectPushyCampaign.PushyCampaignID = this.PushyCampaign.DirectPushyCampaign.ID();
			this.DirectPushyCampaign.LinkID = this.PushyCampaign.Link.DirectLink.LinkID;
			this.DirectPushyCampaign.TrafficNetworkID = this.TrafficCampaign.TrafficSource.DirectModelID;
			this.DirectPushyCampaign.TrafficNetworkName = this.TrafficCampaign.TrafficNetworkName;
			this.DirectPushyCampaign.TrafficNetworkCampaignID = this.TrafficCampaign.DirectModelID.Value;
			this.DirectPushyCampaign.LinkName = string.Format("{0}.{1}", this.PushyCampaign.Link.FullName, this.TrafficCampaign.TrafficNetworkName);
			this.DirectPushyCampaign.CampaignID = this.CampaignID;
			this.DirectPushyCampaign.Budget = this.Budget;
			this.DirectPushyCampaign.ClicksForConversion = this.ClickskPerConversion;

			this.DirectPushyCampaign.InitialClicks = this.InitialAfflowStats.Clicks;	// initial data
			this.DirectPushyCampaign.InitialConversions = this.InitialAfflowStats.Conversions;
			this.DirectPushyCampaign.InitialCost = this.InitialTrafficStats.Spent;
			this.DirectPushyCampaign.InitialPayout = this.InitialAfflowStats.Revenue;
			PushyCashDirect.Instance.Insert(this.DirectPushyCampaign);
						
			// check traffic source daily budget
			if(this.TrafficCampaign.TrafficSource.Spent > this.TrafficCampaign.TrafficSource.DailyBudget)
			{ 
				this.Stop($"Campaign will not start. Traffic source spen {Math.Round(this.TrafficCampaign.TrafficSource.Spent, 2)}$ and daily budget is {Math.Round(this.TrafficCampaign.TrafficSource.DailyBudget, 2)}$");
				return;
			}
			
			this.TrafficCampaign.Reload();
			
			if (this.CheckIfThereIsRisk())
				return;

			this.WebModel = new PushyNetworkCampaignWebModel()
			{
				LinkName = this.PushyCampaign.Link.FullName,
				Name = this.TrafficCampaign.TrafficNetworkName,
				NetworkName = this.TrafficCampaign.TrafficNetworkName,
				Budget = Math.Round(this.Budget, 2),
				ToleratedDeficit = Math.Round(this.ToleratedDeficit, 2),
				ClicksForConversion = this.ClickskPerConversion,
				Started = DateTime.Now
			};
			Web.Core.PushyWebSender.Send<PushyNetworkCampaignWebModel>("OnPushyNetworkCampaignStart", this.WebModel);
			this.StatusEventChange = new PushyNetworkCampaignWebModelStatusChanger(this);

			this.Log("Starting...");

			this.Created = DateTime.Now;
			this.AutoBidder();
			this.Start();
			this.TryOptimization();
		}
		public void LoadInitialCampaignStats()
		{
			// get afflow initial results
			this.InitialAfflowInformations = PushyContext.AfflowManager.GetInitialCampaignData(this.NID, this.CampaignID, this.TrafficCampaign.Country, this.PushyCampaign.Link.AfflowMobileOperator, this.TrafficCampaign.Device);
			Afflow.Models.AfflowCampaignStat initialCampaignInfo = InitialAfflowInformations.CampaignStats;
			if (initialCampaignInfo != null)
			{
				this.InitialAfflowStats.Clicks = initialCampaignInfo.Clicks;
				this.InitialAfflowStats.Conversions = initialCampaignInfo.Leads;
				this.InitialAfflowStats.Revenue = initialCampaignInfo.Revenue;
			}
			
			TrafficNetworks.Core.TrafficCurrentStats trafficCurrentStats = this.TrafficCampaign.GetCurrentStats();
			if (trafficCurrentStats != null)
			{
				this.InitialTrafficStats.Clicks = trafficCurrentStats.Clicks;
				this.InitialTrafficStats.Conversions = trafficCurrentStats.Conversions;
				this.InitialTrafficStats.Spent = trafficCurrentStats.Spent;
			}
		}
		public bool CheckIfThereIsRisk()
		{
			NetworkCampaignMetrics networkCampaignMetrics = new NetworkCampaignMetrics(this);
			if (!networkCampaignMetrics.ShouldCampaignRun)
				this.Stop(networkCampaignMetrics.Explanation);

			this.Log("NetworkMetric multiplier is " + networkCampaignMetrics.Multiplier);

			this.Budget *= networkCampaignMetrics.Multiplier;
			if (this.Budget > PushyGlobal.InitialConfiguration.MaximumBudget)
				this.Budget = PushyGlobal.InitialConfiguration.MaximumBudget;

			this.DirectPushyCampaign.Budget = this.Budget;
			PushyCashDirect.Instance.Update(this.DirectPushyCampaign);

			return false;
		}
		public void Start()
		{
			PLogger.Debug($"Campaign started {this.PushyCampaign.Link.Name}.{this.TrafficCampaign.TrafficNetworkName}");
			DirectActiveCampaign.Insert(this.CampaignID, this.TrafficCampaign.TrafficNetworkName);
			this.TrafficCampaign.Start();
		}
		public void Stop(string explanationWhy = "")
		{
			PLogger.Debug($"Link {this.PushyCampaign.Link.FullName} is stoping network {this.TrafficCampaign.TrafficNetworkName} due: {explanationWhy}");
			this.DirectPushyCampaign.Conversions = this.Conversions;
			this.DirectPushyCampaign.Clicks = this.Clicks;
			this.DirectPushyCampaign.Cost = this.Spent;
			this.DirectPushyCampaign.Payout = this.Payout;
			this.DirectPushyCampaign.LastBid = this.TrafficCampaign.BidValue;
			this.DirectPushyCampaign.ReasonForClosing = explanationWhy;
			this.DirectPushyCampaign.HadClicks = this.Clicks > 0;
			this.DirectPushyCampaign.HadConversions = this.Conversions > 0;
			this.DirectPushyCampaign.WasProfitable = this.Payout > 0.1;
			this.DirectPushyCampaign.WasFinished = true;
			this.DirectPushyCampaign.Ended = DateTime.Now;
			this.DirectPushyCampaign.ROI = (this.Payout == 0) ? -100.0 : Math.Round((this.Payout - this.Spent) / this.Spent * 100);
			PushyCashDirect.Instance.Update(this.DirectPushyCampaign);

			this.TrafficCampaign.LastDirectPushyNetworkCampaign = this.DirectPushyCampaign;
			
			Web.Core.PushyWebSender.Send<PushyNetworkCampaignWebModel>("OnPushyNetworkCampaignStop", this.WebModel);

			this.Log(explanationWhy);
			this.IsStoped = true;
			DirectActiveCampaign.Delete(this.CampaignID, this.TrafficCampaign.TrafficNetworkName);
			this.TrafficCampaign.Stop();
			this.TrafficCampaign.ResumeTemporaryStopedTargets();
		}
		public void Pause(string explanationWhy = "")
		{
			this.Log(explanationWhy);
			//this.PausedTimeout = DateTime.Now.AddHours(1);
			this.PausedTimeout = DateTime.Now.AddMinutes(1);
			PLogger.Debug($"Campaign paused {this.PushyCampaign.Link.Name}.{this.TrafficCampaign.TrafficNetworkName}");
			this.Log(explanationWhy);
			this.TrafficCampaign.Stop();
		}
		public void Update()
		{
			this.StatusEventChange.SendReportIfThereAreChanges();
			if (this.IsStoped)
				return;
			
			if (PushyGlobal.CurrentRevenue <= (PushyGlobal.DailyBudget * -1))
			{
				this.Stop("Stoping because day budget is lost");
				return;
			}
						
			if (this.Conversions == 0 && this.Clicks == 0)
				return;

			#region # (closing) Main closing logic... #

			if(!PushyContext.AfflowManager.CheckIfFilteredOfferIsReceivingTraffic(this.TrafficCampaign.TrafficSource.AfflowMap.NID, this.TrafficCampaign.ID, this.PushyCampaign.OffersActivated))
			{
				this.Stop($"Stoping because Afflow is not sending traffic to " + this.PushyCampaign.OffersWhitelisted);
				return;
			}

			// conversions happened but we are on minus, so close 
			if (this.Conversions >= 3 && this.EarnedMoney < (this.ToleratedDeficit * -1))
			{
				this.Stop($"Stoping because low preformace (Conversions={this.Conversions} and ToleratedDeficit={Math.Round(this.ToleratedDeficit, 2)})");
				return;
			}
			
			if (this.Conversions >= 0 && this.Conversions < 3 && this.EarnedMoney < (this.Budget * -1))
			{
				this.Stop($"Stoping because low preformace (Conversions={this.Conversions} and EarnedMoney={Math.Round(this.EarnedMoney, 2)})");
				return;
			}

			if (this.Conversions == 0 && (DateTime.Now - this.Created).TotalMinutes >= 90)
			{
				this.Stop($"Stoping because in 90min there are no conversions");
				return;
			}

			int expectedConversions = (int)Math.Floor((this.Clicks * 1.0) / (this.ClickskPerConversion * 1.0));
			if (this.Conversions < expectedConversions)
			{
				this.Stop($"Stoping because conversions({this.Conversions}) are lower than expected ({expectedConversions})");
				return;
			}

			#endregion
			
			this.TryOptimization();

		}
		public void TryOptimization()
		{
			if (!this.LastOptimizationChecked.HasValue ||(DateTime.Now - this.LastOptimizationChecked.Value).TotalMinutes >= 10)
			{
				List<TrafficTargetsInfo> targets = new List<TrafficTargetsInfo>();

				foreach (var afflowTargetStat in PushyContext.AfflowManager.GetTrafficTargetInfo(this.TrafficCampaign.TrafficSource.AfflowMap.NID, this.TrafficCampaign.ID))
				{

					if(this.InitialTrafficTargetReport != null)
					{
						TrafficTargetsInfo tti = (from t in this.InitialTrafficTargetReport where t.Name.Equals(afflowTargetStat.Keyword) select t).FirstOrDefault();
						afflowTargetStat.Clicks -= tti.Clicks;
						afflowTargetStat.Conversions -= tti.Conversions;
					}

					targets.Add(new TrafficTargetsInfo()
					{
						Name = afflowTargetStat.Keyword,
						Clicks = afflowTargetStat.Clicks,
						Conversions = afflowTargetStat.Conversions,
						BotRisk = afflowTargetStat.BotRisk
					});
				}

				TrafficOptimizationResult result = this.TrafficCampaign.TryOptimization(targets, initialRun:(!LastOptimizationChecked.HasValue));
				if (result == TrafficOptimizationResult.RemovedPermanently)
					this.Log("Optimization removed some targets permanently");
				else if(result == TrafficOptimizationResult.RemovedTemporary)
					this.Log("Optimization removed some targets temporary");
				else if( result == TrafficOptimizationResult.RemovedTemporaryAndPermanently)
					this.Log("Optimization removed some targets temporary and some permanently");

				if (this.InitialTrafficTargetReport == null)
					this.InitialTrafficTargetReport = targets;

				this.LastOptimizationChecked = DateTime.Now;
			}
		}
		public void AutoBidder()
		{
			if (!this.TrafficCampaign.TrafficSource.SupportsAutoBid)
				return;

			//double? defaultBidValue = PushyCashDirect.Instance.LoadDouble(
			//	@"SELECT TOP 1 LastBid FROM [].PushyNetworkCampaign WHERE CampaignID={0} AND TrafficNetworkName={1} ORDER BY PushyNetworkCampaignID DESC", this.CampaignID, this.TrafficCampaign.TrafficNetworkName);
			//if (!defaultBidValue.HasValue)
			//	defaultBidValue = 0.0001;


			double? defaultBidValue = 0.0401;
			this.TrafficCampaign.Bid(defaultBidValue.Value);
			this.DirectPushyCampaign.LastBid = defaultBidValue.Value;
			PushyCashDirect.Instance.Update(this.DirectPushyCampaign);
			this.Log(string.Format("Bid starts with {0}", this.DirectPushyCampaign.LastBid.Value));

			this.TrafficCampaign.Reload();
			PushyContext.TrafficNetworkManager.SaveStorageTrafficData();
			
			new Thread(() =>
			{
				for(; ;)
				{
					Thread.Sleep(15 * 1000);
					if (this.IsStoped)
						break;

					if(this.TrafficCampaign.AutoBid())
					{
						this.Log(string.Format("Bid changed from {0} to {1}", Math.Round(this.DirectPushyCampaign.LastBid.Value, 4), Math.Round(this.TrafficCampaign.BidValue, 4)));
						this.DirectPushyCampaign.LastBid = this.TrafficCampaign.BidValue;
						PushyCashDirect.Instance.Update(this.DirectPushyCampaign);
					}
				}
			}).Start();
		}
		public void Log(string text)
		{
			Web.Core.PushyWebSender.Send<PushyNetworkCampaignLog>("OnPushyNetworkCampaignLog", new PushyNetworkCampaignLog()
			{
				LinkName = this.PushyCampaign.Link.FullName,
				PushyNetworkCampaignName = this.TrafficCampaign.TrafficNetworkName,
				Text = text,
				Created = DateTime.Now
			});
			
			this.Logs.Insert(0, new PushyNetworkCampaignLog() { Text = text });
			if (this.Logs.Count > 5)
				for (int i = 5; i < this.Logs.Count - 1; i++)
					this.Logs.RemoveAt(i);

			this.DirectPushyCampaign.Log(text);
		}
	}
}
