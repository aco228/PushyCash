using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Slack
{
	public class SlackGeneral : SlackChannel
	{
		internal SlackGeneral(SlackApi api) : base(api, "https://hooks.slack.com/services/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") { }
		
		public void New(string text)
		{
			this.Send(text);
		}



	}
}
