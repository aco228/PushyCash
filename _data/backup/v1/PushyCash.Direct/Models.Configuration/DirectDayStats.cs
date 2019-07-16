using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Direct.Models.Configuration
{
	public class DirectDayStats : DirectModel
	{
		public DirectDayStats() : base("DayStats") { }

		public int DayStatsID { get; set; } = -1;
		public DateTime Day { get; set; }
		public int TrackedConversions { get; set; } = 0;
		public double TrackedPayout { get; set; } = 0.0;
		public int UntrackedConversions { get; set; } = 0;
		public double UntrackedPayout { get; set; } = 0.0;
		public double Payout { get; set; } = 0.0;
		public double Costs { get; set; } = 0.0;
		public int Clicks { get; set; } = 0;
		public int Conversions { get; set; } = 0;

		public static DirectDayStats Load()
		{
			DirectDayStats result = new DirectDayStats();
			result.Day = DateTime.Today;
			DirectContainer load = PushyCashDirect.Instance.LoadContainer("SELECT TOP 1 * FROM  [].DayStats WHERE Day={0}", result.Day);
			if (load.HasValue)
				result = load.Convert<DirectDayStats>();
			if (!load.HasValue)
				result.DayStatsID = PushyCashDirect.Instance.Execute("INSERT INTO [].DayStats (Day)", result.Day).Value;
			return result;
		}

	}
}
