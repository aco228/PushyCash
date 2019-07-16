using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Slack
{
	public class SlackCampaign : SlackChannel
	{
		internal SlackCampaign(SlackApi api) : base(api, "https://hooks.slack.com/services/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") { }
		


		public void Start(string country, string mobileOperator, string vertical, string device, string offer, string networks)
		{
			string text = string.Format("*[START]* `{0} {1} {2} {3} ({4})`", country, mobileOperator, vertical, device, offer) + Environment.NewLine;
			text += "> " + networks;
			this.Send(text);
		}

		public void Report(string country, string mobileOperator, string vertical, string device, string offer, string[] networks)
		{
			string text = string.Format("----> *[REPORT]* `{0} {1} {2} {3} ({4})`", country, mobileOperator, vertical, device, offer) + Environment.NewLine;
			foreach(string n in networks)
				text += "> " + n + Environment.NewLine;
			this.Send(text);
		}

		public void End(string country, string mobileOperator, string vertical, string device, string offer, string[] networks)
		{
			string text = string.Format("*[END]* `{0} {1} {2} {3} ({4})`", country, mobileOperator, vertical, device, offer) + Environment.NewLine;
			foreach (string n in networks)
				text += "> " + n + Environment.NewLine;
			this.Send(text);
		}

		public void NoTrafficNetwork(string country, string mobileOperator, string vertical, string device)
		{
			string text = string.Format("{0} {1} {2} {3}", country, mobileOperator, vertical, device) + " " + "`Link ready but no traffic connected`";
			this.Send(text);
		}


	}
}
