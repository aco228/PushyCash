using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Workers.SingleThreadWorkers
{
	public class SlackReporter : SingleThreadOperationBase
	{
		public SlackReporter() : base(delayInSeconds: 60 * 60 )
		{ }

		protected override void OnCall()
		{
			PushyContext.Slack.Report.Send(PushyGlobal.Conversions, PushyGlobal.Clicks, PushyGlobal.Payout, PushyGlobal.Costs, PushyGlobal.Revenue);
		}
	}
}
