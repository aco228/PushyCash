using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Web.Core.Models
{
  [Serializable()]
  public class MainInformationsWebModel
	{
		public DateTime ServerStarted { get; set; } = DateTime.Now;

		public int TrackedConvertions { get; set; } = 0;
		public int UntrackedConversions { get; set; } = 0;
		public double TrackedPayout { get; set; } = 0.0;
		public double UntrackedPayout { get; set; } = 0.0;

		public int Clicks { get; set; } = 0;
		public int Conversions { get; set; } = 0;

		public double TodayMaximumBudget { get; set; } = 0.0;
		public double TodayPayout { get; set; } = 0.0;
		public double TodayRavenue { get => Math.Round(this.TodayPayout - this.TodayCosts, 2); }
		public double TodayCosts { get; set; } = 0.0;
    public string ServerCreatedTime { get => ServerStarted.ToString("HH:mm"); }

		public double ROI { get => this.TodayCosts== 0 || this.TodayRavenue == 0 ? 0.0 : Math.Round(this.TodayCosts / this.TodayRavenue * 100, 2); }
  }
}
