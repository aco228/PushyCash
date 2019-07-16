using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Slack
{
	public class SlackReport : SlackChannel
	{
		internal SlackReport(SlackApi api) : base(api, "https://hooks.slack.com/services/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")
		{
		}

		public void Send(int conversions, int clicks, double payout, double cost, double revenue)
		{
			this.API.Send(this.URL, 
				$"Conversions/Clicks: *{conversions}/{clicks}*" + Environment.NewLine
				+ $"Payout/Cost: *{Math.Round(payout, 2)}/{Math.Round(cost, 2)}*" + Environment.NewLine
				+ $"Revenue: *{Math.Round(revenue, 2)}*" + Environment.NewLine);
		}
	}
}
