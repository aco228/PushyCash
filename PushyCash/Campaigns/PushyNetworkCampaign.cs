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
		private DateTime _lastAfflowTrafficOfferChecked = DateTime.Now; // timeout for checking if traffic is sent to whitelisted offers

		public PushyCampaign PushyCampaign { get; protected set; } = null;
		public DateTime? LastOptimizationChecked { get; protected set; } = null;
		public Dictionary<string, TrafficTargetsInfo> InitialTrafficTargetReport { get; protected set; } = null; // keyword is the key
		public AfflowInitialCampaignData InitialAfflowInformations { get; protected set; } = null;
		public List<PushyNetworkCampaignLog> Logs { get; protected set; } = new List<PushyNetworkCampaignLog>();
		public PushyNetworkCampaignWebModel WebModel { get; set; } = null;
		public PushyNetworkCampaignWebModelStatusChanger StatusEventChange { get; set; } = null;

		public TrafficCampaign TrafficCampaign { get; protected set; } = null;
		public DirectPushyNetworkCampaign DirectPushyCampaign { get; protected set; } = null;
		public string CampaignID { get => TrafficCampaign.ID; }
		public string NID { get => TrafficCampaign.Afflowlink.nid; }
		public bool IsStoped { get; protected set; } = false;
		public DateTime Created { get; protected set; } = DateTime.Now;
		
		public int ClickskPerConversion { get; set; } 
		public double Budget { get; private set; } = 0.0;
		public double ToleratedDeficit { get => this.PushyCampaign.AllowedDeficit; }
		
		public int Clicks { get => (this.DirectPushyCampaign.InitialClicks == 0 ? this.TrafficCampaign.Stats.Clicks : this.TrafficCampaign.Stats.Clicks -  this.DirectPushyCampaign.InitialClicks); }
		public int Conversions { get => (this.DirectPushyCampaign.InitialConversions == 0 ? this.TrafficCampaign.Stats.Conversions : this.TrafficCampaign.Stats.Conversions - this.DirectPushyCampaign.InitialConversions); }
		public double Payout { get => (this.DirectPushyCampaign.InitialPayout == 0 ? this.TrafficCampaign.Stats.Payout : this.TrafficCampaign.Stats.Payout - this.DirectPushyCampaign.InitialPayout); }
		public double Spent { get => (this.DirectPushyCampaign.InitialCosts == 0 ? this.TrafficCampaign.Stats.Costs : this.TrafficCampaign.Stats.Costs - this.DirectPushyCampaign.InitialCosts); }
		public double EarnedMoney { get => this.Payout - this.Spent; }
		public double ROI => this.Payout == 0 || this.Spent == 0 ? -100.0 : (((this.Payout - this.Spent) / this.Spent) * 100);
		public double? Bid { get => TrafficCampaign.BidValue; }
		
		public PushyNetworkCampaign(PushyCampaign pushyCampaign, TrafficCampaign campaign)
		{
			this.PushyCampaign = pushyCampaign;
			this.TrafficCampaign = campaign;
			this.Budget = this.PushyCampaign.Metrics.LinkRecomendedBudget;
			this.ClickskPerConversion = this.PushyCampaign.Metrics.LinkRecomendedClicksWithoutConversion;
			PLogger.Debug($"Link {pushyCampaign.Link.FullName} is starting network {campaign.TrafficNetworkName}");
						
			this.DirectPushyCampaign = new DirectPushyNetworkCampaign()
			{
				PushyCampaignID = this.PushyCampaign.DirectPushyCampaign.ID(),
				LinkID = this.PushyCampaign.Link.DirectLink.LinkID,
				LinkName = this.PushyCampaign.Link.FullName,
				TrafficNetworkID = this.TrafficCampaign.TrafficSource.DirectModelID,
				TrafficNetworkCampaignID = this.TrafficCampaign.DirectModelID.Value,
				TrafficCampaignDayStatID = this.TrafficCampaign.Stats.ID(),
				TrafficNetworkName = this.TrafficCampaign.TrafficNetworkName,
				Budget = this.Budget,
				ClicksForConversion = this.ClickskPerConversion,

				InitialClicks = this.TrafficCampaign.Stats.Clicks,
				InitialConversions = this.TrafficCampaign.Stats.Conversions,
				InitialPayout = this.TrafficCampaign.Stats.Payout,
				InitialCosts = this.TrafficCampaign.Stats.Costs
			};
			PushyCashDirect.Instance.Insert(this.DirectPushyCampaign);

			this.TrafficCampaign.Stats.IsRunning = true;
			this.TrafficCampaign.Stats.NumberOfTries++;
			this.TrafficCampaign.Stats.Update();

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

			// check traffic source daily budget
			if (this.TrafficCampaign.TrafficSource.Spent > this.TrafficCampaign.TrafficSource.DailyBudget)
			{ 
				this.Stop($"Campaign will not start. Traffic source spen {Math.Round(this.TrafficCampaign.TrafficSource.Spent, 2)}$ and daily budget is {Math.Round(this.TrafficCampaign.TrafficSource.DailyBudget, 2)}$");
				return;
			}

			if(this.TrafficCampaign.Stats.Revenue < (PushyGlobal.InitialConfiguration.MaximumBudgetForTrafficCampaign * -1))
			{
				this.Stop($"Campaign will not start. Traffic campaign spent {Math.Round(this.TrafficCampaign.Stats.Revenue, 2)}$ and daily budget for TrafficNetworkCampaign are {Math.Round(PushyGlobal.InitialConfiguration.MaximumBudgetForTrafficCampaign, 2)}$");
				return;
			}
			
			this.TrafficCampaign.Reload();
			
			if (this.CheckIfThereIsRisk())
				return;
			
			this.Log("Starting...");

			this.Created = DateTime.Now;
			this.AutoBidder();
			this.Start();
			this.TryOptimization();
		}
		public bool CheckIfThereIsRisk()
		{
			NetworkCampaignMetrics networkCampaignMetrics = new NetworkCampaignMetrics(this);
			if (!networkCampaignMetrics.ShouldCampaignRun)
			{
				this.Stop(networkCampaignMetrics.Explanation);
				return true;
			}

			TrafficCampaignStartMetrics trafficCampaignMetrics = this.TrafficCampaign.CheckIfThereIsRisk();
			if(!trafficCampaignMetrics.ShouldCampaignRun)
			{
				this.Stop(trafficCampaignMetrics.Explanation);
				return true;
			}

			this.Log("NetworkMetric multiplier is " + networkCampaignMetrics.Multiplier);

			this.Budget *= networkCampaignMetrics.Multiplier;
			if (this.Budget > PushyGlobal.InitialConfiguration.MaximumBudget)
				this.Budget = PushyGlobal.InitialConfiguration.MaximumBudget;
      if (this.Budget < 0.20)
        this.Budget = 0.20;
			
			this.DirectPushyCampaign.Budget = this.Budget;
			this.DirectPushyCampaign.Update();

			return false;
		}
		public void Start()
		{
			PLogger.Debug($"Campaign started {this.PushyCampaign.Link.Name}.{this.TrafficCampaign.TrafficNetworkName}");
			this.TrafficCampaign.Start();
		}
		public void Stop(string explanationWhy = "")
		{
			PLogger.Debug($"Link {this.PushyCampaign.Link.FullName} is stoping network {this.TrafficCampaign.TrafficNetworkName} due: {explanationWhy}");
			this.DirectPushyCampaign.Conversions = this.Conversions;
			this.DirectPushyCampaign.Clicks = this.Clicks;
			this.DirectPushyCampaign.Costs = this.Spent;
			this.DirectPushyCampaign.Payout = this.Payout;
			this.DirectPushyCampaign.LastBid = this.TrafficCampaign.BidValue;
			this.DirectPushyCampaign.ReasonForClosing = explanationWhy;
			this.DirectPushyCampaign.Ended = DateTime.Now;
			this.DirectPushyCampaign.Update();
			this.DirectPushyCampaign.Update();

			Web.Core.PushyWebSender.Send<PushyNetworkCampaignWebModel>("OnPushyNetworkCampaignStop", this.WebModel);

			this.Log(explanationWhy);
			this.IsStoped = true;
			this.TrafficCampaign.Stop();
		}
		public void Update()
		{
			this.StatusEventChange.SendReportIfThereAreChanges();
			if (this.IsStoped)
			{
				// TODO: revisit this
				//if(this.TrafficCampaign.Conversions > 0 && this.TrafficCampaign.Stats.ROI > -78 && this.TrafficCampaign.Stats.Revenue >= (this.Budget * -1))
				//{
				//	// give another try for delayed conversions (if ROI is above -50% and budget still exists)
				//	this.ClickskPerConversion += 1000;
				//	this.Budget += 0.2;
				//	this.Log("Restarting camp becase of conv and roi (adding 1000c and 0.2$ budget)");
				//	this.Start();
				//}
				return;
			}
			
			if (PushyGlobal.CurrentRevenue <= (PushyGlobal.DailyBudget * -1))
			{
				this.Stop("Stoping because day budget is lost");
				return;
			}
						
			if (this.Conversions == 0 && this.Clicks == 0)
				return;

			#region # (closing) Main closing logic... #

			if(this.TrafficCampaign.Stats.Revenue < (PushyGlobal.InitialConfiguration.MaximumBudgetForTrafficCampaign * -1))
			{
				this.Stop($"Stoping because maximum budget for campaing:{PushyGlobal.InitialConfiguration.MaximumBudgetForTrafficCampaign} is waisted");
				return;
			}

			if(this.Clicks > 30 
				&& (DateTime.Now - this._lastAfflowTrafficOfferChecked).TotalMinutes >= 2 && !PushyContext.AfflowManager.CheckIfFilteredOfferIsReceivingTraffic(this.TrafficCampaign.TrafficSource.AfflowMap.NID, this.TrafficCampaign.ID, this.PushyCampaign.OffersActivated)
				&& this.TrafficCampaign.Stats.ROI < -50)
			{
				this._lastAfflowTrafficOfferChecked = DateTime.Now;
				this.Stop($"Stoping because Afflow is not sending traffic to " + this.PushyCampaign.OffersWhitelisted);
				return;
			}

			// conversions happened but we are on minus, so close 
			if (this.Conversions >= PushyGlobal.InitialConfiguration.MaximumConversionsForDeficitTracking && this.EarnedMoney < (this.ToleratedDeficit * -1))
			{
				this.Stop($"Stoping because low preformace (Conversions={this.Conversions} and ToleratedDeficit={Math.Round(this.ToleratedDeficit, 2)})");
				return;
			}
			
			if (this.Conversions >= 0 && this.Conversions < 3 && this.EarnedMoney < (this.Budget * -1))
			{
				this.Stop($"Stoping because low preformace (Conversions={this.Conversions} and EarnedMoney={Math.Round(this.EarnedMoney, 2)})");
				return;
			}

			if (this.Conversions == 0 && (DateTime.Now - this.Created).TotalMinutes >= PushyGlobal.InitialConfiguration.MaximumMinutesForCampaignWithNoConversions)
			{
				this.Stop($"Stoping because in {PushyGlobal.InitialConfiguration.MaximumMinutesForCampaignWithNoConversions}min there are no conversions");
				return;
			}

			if (this.TrafficCampaign.Stats.Revenue < (PushyGlobal.InitialConfiguration.MaximumBudgetForTrafficCampaign * -1))
			{
				this.Stop($"Stoping because traffic campaign spent {Math.Round(this.TrafficCampaign.Stats.Revenue, 2)}$ and daily budget for TrafficNetworkCampaign are {Math.Round(PushyGlobal.InitialConfiguration.MaximumBudgetForTrafficCampaign, 2)}$");
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
			return;
			if (LastOptimizationChecked.HasValue && this.Clicks < 15)
				return;

			if (!this.LastOptimizationChecked.HasValue ||(DateTime.Now - this.LastOptimizationChecked.Value).TotalMinutes >= PushyGlobal.InitialConfiguration.OptimizationMinutesTimeout)
			{
				List<TrafficTargetsInfo> targets = new List<TrafficTargetsInfo>();

				if (this.InitialTrafficTargetReport == null)
					this.InitialTrafficTargetReport = new Dictionary<string, TrafficTargetsInfo>();

				foreach (var afflowTargetStat in PushyContext.AfflowManager.GetTrafficTargetInfo(this.TrafficCampaign.TrafficSource.AfflowMap.NID, this.TrafficCampaign.ID))
				{
					var tti = (this.InitialTrafficTargetReport.ContainsKey(afflowTargetStat.Keyword)) ? this.InitialTrafficTargetReport[afflowTargetStat.Keyword] : null;
					if(tti == null)
					{
						tti = new TrafficTargetsInfo() { Name = afflowTargetStat.Keyword, Clicks = afflowTargetStat.Clicks, Conversions = afflowTargetStat.Conversions };
						this.InitialTrafficTargetReport.Add(tti.Name, tti);
					}

					afflowTargetStat.CorrectedClicks -= tti.Clicks;
					afflowTargetStat.Conversions -= tti.Conversions;
					
					targets.Add(new TrafficTargetsInfo()
					{
						Name = afflowTargetStat.Keyword,
						Clicks = afflowTargetStat.Clicks,
						CorrectedClicks = afflowTargetStat.CorrectedClicks,
						Conversions = afflowTargetStat.Conversions,
						BotRisk = afflowTargetStat.BotRisk
					});
				}

				TrafficOptimizationResult result = this.TrafficCampaign.RunOptimization(targets, initialRun:(!LastOptimizationChecked.HasValue));
				if (result == TrafficOptimizationResult.RemovedPermanently)
					this.Log("Optimization removed some targets permanently");
				else if(result == TrafficOptimizationResult.RemovedTemporary)
					this.Log("Optimization removed some targets temporary");
				else if( result == TrafficOptimizationResult.RemovedTemporaryAndPermanently)
					this.Log("Optimization removed some targets temporary and some permanently");
				
				this.LastOptimizationChecked = DateTime.Now;
			}
		}
		public void AutoBidder()
		{
			if (!this.TrafficCampaign.TrafficSource.SupportsAutoBid)
				return;

			this.TrafficCampaign.BeforeAutoBiddingStarts();
			this.DirectPushyCampaign.LastBid = this.TrafficCampaign.BidValue;
			this.DirectPushyCampaign.Update();
			this.Log(string.Format("Bid starts with {0}", this.DirectPushyCampaign.LastBid.Value));
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
						this.Log(string.Format("Bid changed from {0} to {1}", Math.Round(this.DirectPushyCampaign.LastBid.Value, 4), Math.Round(this.TrafficCampaign.BidValue.Value, 4)));
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

			this.TrafficCampaign.Stats.Log(text);
			this.DirectPushyCampaign.Log(text);
		}
	}
}
