using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Slack
{
	public abstract class SlackChannel
	{
		internal SlackApi API { get; private set; } = null;
		internal string URL { get; private set; } = string.Empty;
		
		internal SlackChannel(SlackApi api, string url)
		{
			this.API = api;
			this.URL = url;
		}

		protected string GetHeader(string country, string mobileOperator, string vertical, string device, string offerID = "")
		{
			return string.Format("*{0}* {1} {2} {3}{4}", country, mobileOperator, vertical, device, (!string.IsNullOrEmpty(offerID) ? "*" + offerID + "*" : "")) + Environment.NewLine;
		}

		protected void Send(string text)
		{
			this.API.Send(this.URL, text);
		}

	}
}
