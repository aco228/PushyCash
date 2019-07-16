using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Slack
{
	public class SlackInterestingOffers : SlackChannel
	{
		internal SlackInterestingOffers(SlackApi api) : base(api, "https://hooks.slack.com/services/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") { }

		public void New(string country, string mobileOperator, string vertical, string device,
			double avarigePayout, string offerID, double minutesPassed, int numberOfTransatcions, int minutes)
		{
			string text = this.GetHeader(country, mobileOperator, vertical, device, offerID);
			text += string.Format("> `({0}min of {1}min) ({2} conversions with avarage payout {3})`",
									minutesPassed,
									minutes,
									numberOfTransatcions,
									avarigePayout);
			this.Send(text);
		}
		
	}
}
