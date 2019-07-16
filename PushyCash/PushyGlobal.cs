using PushyCash.Direct.Models.Configuration;
using PushyCash.InitialConfigurations.TrackingOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash
{
	public static class PushyGlobal
	{
		private static PushyInitialConfiguration _initialConfiguration = null;
		private static DirectMainConfiguration _mainConfiguration = null;
		private static PushyTrackingOptions _pushyTrackingOptions = null;

		public static DirectMainConfiguration MainConfiguration
		{
			get
			{
				if (_mainConfiguration != null)
					return _mainConfiguration;
				_mainConfiguration = DirectMainConfiguration.Load();
				return _mainConfiguration;
			}
		}
		public static PushyInitialConfiguration InitialConfiguration
		{
			get
			{
				if (_initialConfiguration != null)
					return _initialConfiguration;
				_initialConfiguration = new PushyInitialConfiguration();
				return _initialConfiguration;
			}
		}
		public static PushyTrackingOptions LinkTrackingOptions
		{
		get
			{
				if (_pushyTrackingOptions != null)
					return _pushyTrackingOptions;
				_pushyTrackingOptions = new PushyTrackingOptions();
				return _pushyTrackingOptions;
			}
		}
		
		public static DirectDayStats DailyStats = DirectDayStats.Load();
		public static DateTime ApplicationStarted = DateTime.Now;

		public static double DailyBudget { get => MainConfiguration.DailyBudget; }
		public static double CurrentRevenue { get => Payout - Costs; }

		public static int TrackedConversions { get => DailyStats.TrackedConversions; set => DailyStats.TrackedConversions = value; }
		public static int UntrackedConversions { get => DailyStats.UntrackedConversions; set => DailyStats.UntrackedConversions = value; }
		public static double TrackedPayout { get => DailyStats.TrackedPayout; set => DailyStats.TrackedPayout = value; }
		public static double UntrackedPayout { get => DailyStats.UntrackedPayout; set => DailyStats.UntrackedPayout = value; }

		public static int Clicks { get => DailyStats.Clicks; set => DailyStats.Clicks = value; }
		public static int Conversions { get => DailyStats.Conversions; set => DailyStats.Conversions = value; }
		public static double Payout { get => DailyStats.Payout; set => DailyStats.Payout = value; }
		public static double Costs { get => DailyStats.Costs; set => DailyStats.Costs = value; }
		public static double Revenue { get => Payout - Costs; }

		public static string ApplicationStartedString { get => $"{(ApplicationStarted.Hour < 10 ? "0" : "") + ApplicationStarted.Hour.ToString() }:{(ApplicationStarted.Minute < 10 ? "0" : "") + ApplicationStarted.Minute.ToString()}"; }
		public static string TimeElapsed { get => (int)Math.Floor((DateTime.Now - ApplicationStarted).TotalMinutes) + "m"; }

		public static string GetDate(DateTime time)
		{
			return $"{(time.Hour < 10 ? "0" : "") + time.Hour.ToString() }:{(time.Minute < 10 ? "0" : "") + time.Minute.ToString()}";
		}

	}
}
