using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Afflow.Models
{
	/*
		This is report where we get informations about targets that make conversions (mostly based on Keyword 3)
		https://api.monetizer.co/data/report.php?nid={0}&rcid={1}&order=clicks&direction=desc&keyword=ts&start_ts={2}&end_ts={3}&tz=Europe/Belgrade

	*/
	public class AfflowTrafficTargetReport
	{
		public string Keyword { get; set; } = string.Empty;
		public int Clicks { get; set; } = 0;
		public int Conversions { get; set; } = 0;
		public int BotClicks { get; set; } = 0;

		public double BotRisk { get => this.Clicks < 300 ? 0 : (this.BotClicks / this.Clicks * 100); }
	}
}
