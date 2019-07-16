using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Direct.Models.Main
{
	public class DirectDayStat : DirectModel
	{
		public DirectDayStat() : base("DayStats") { }

		public int DayStatsID { get; set; } = -1;
		public string Day { get; set; } = string.Empty;
		public double Payout { get; set; } = 0.0;
		public double Costs { get; set; } = 0.0;

		public static DirectDayStat Load()
		{
			return PushyCashDirect.Instance.LoadContainer("SELECT TOP 1 * FROM [].DayStats WHERE Day={0} ORDER BY DayStatsID DESC", DirectHelper.GetDateTime(DateTime.Today)).Convert<DirectDayStat>();
		}
	}
}
