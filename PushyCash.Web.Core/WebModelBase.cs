using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Web.Core
{
	[Serializable()]
	public class WebModelBase
	{
		private string _action = string.Empty;

		public WebModelBase(string action)
		{
			this._action = action;
		}

		public void Send()
		{
			Web.Core.PushyWebSender.Send(this._action, this);
		}


	}
}
