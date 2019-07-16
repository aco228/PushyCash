using Direct.Core;
using PushyCash.Direct;
using PushyCash.Direct.Models.Configuration;
using PushyCash.Web.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Workers.SingleThreadWorkers
{
	public class GetCostsAndPayouts : SingleThreadOperationBase
	{
		public GetCostsAndPayouts() : base(delayInSeconds:15, executeImmediately:true) { }

		protected override void OnCall()
		{

			PushyGlobal.Clicks = PushyContext.TrafficNetworkManager.Clicks;
			PushyGlobal.Conversions = PushyContext.TrafficNetworkManager.Conversions;
			PushyGlobal.Costs = PushyContext.TrafficNetworkManager.Spent;
			PushyGlobal.Payout = PushyContext.TrafficNetworkManager.Payout;

			Web.Core.PushyWebSender.CallBackupDeamon();
			Web.Core.PushyWebSender.Send<MainInformationsWebModel>(
				"MainInformations",
				new MainInformationsWebModel()
				{
					ServerStarted = PushyGlobal.ApplicationStarted,
					TrackedConvertions = PushyGlobal.TrackedConversions,
					UntrackedConversions = PushyGlobal.UntrackedConversions,
					TrackedPayout = Math.Round(PushyGlobal.TrackedPayout, 2),
					UntrackedPayout = Math.Round(PushyGlobal.UntrackedPayout, 2),
					TodayMaximumBudget = Math.Round(PushyGlobal.DailyBudget, 2),
					TodayPayout = Math.Round(PushyGlobal.Payout, 2),
					TodayCosts = Math.Round(PushyGlobal.Costs, 2),
					Clicks = PushyGlobal.Clicks,
					Conversions = PushyGlobal.Conversions
				});

			if (DateTime.Today != PushyGlobal.DailyStats.Day)
				PushyGlobal.DailyStats = DirectDayStats.Load();
			PushyCashDirect.Instance.Update(PushyGlobal.DailyStats);

			PConsole.PrintHeader();

			//-------------------------------------------------------------------------------------

			//double costs = 0.0, payout = 0.0;
			//int clicks = 0, conversions = 0;

			//// get costs from networks
			//foreach (var tn in PushyContext.TrafficNetworkManager.NetworksList)
			//{
			//	var stat = tn.GetTodayStats();
			//	if (stat == null)
			//		continue;

			//	costs += stat.Spent;
			//	clicks += stat.Redirects;
			//	conversions = stat.Conversions;
			//}

			//Afflow.Models.AfflowRevenue revenue = PushyContext.AfflowManager.GetTodayRevenue();

			//if (revenue != null)
			//	PushyGlobal.Payout = revenue.Ravenue;

			//PushyGlobal.Clicks = clicks;
			//PushyGlobal.Conversions = conversions;
			//PushyGlobal.Costs = costs;

			//Web.Core.PushyWebSender.CallBackupDeamon();
			//Web.Core.PushyWebSender.Send<MainInformationsWebModel>(
			//	"MainInformations",
			//	new MainInformationsWebModel()
			//	{
			//		ServerStarted = PushyGlobal.ApplicationStarted,
			//		TrackedConvertions = PushyGlobal.TrackedConversions,
			//		UntrackedConversions = PushyGlobal.UntrackedConversions,
			//		TrackedPayout = Math.Round(PushyGlobal.TrackedPayout, 2),
			//		UntrackedPayout = Math.Round(PushyGlobal.UntrackedPayout, 2),
			//		TodayMaximumBudget = Math.Round(PushyGlobal.DailyBudget, 2),
			//		TodayPayout = Math.Round(PushyGlobal.Payout, 2),
			//		TodayCosts = Math.Round(PushyGlobal.Costs, 2),
			//		Clicks = PushyGlobal.Clicks,
			//		Conversions = PushyGlobal.Conversions
			//	});

			//if (DateTime.Today != PushyGlobal.DailyStats.Day)
			//	PushyGlobal.DailyStats = DirectDayStats.Load();
			//PushyCashDirect.Instance.Update(PushyGlobal.DailyStats);

			//PConsole.PrintHeader();
		}
	}
}
