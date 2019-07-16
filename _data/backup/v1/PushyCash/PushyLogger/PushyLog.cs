using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PushyCash.PLogger;

namespace PushyCash.PushyLogger
{
	public class PushyLog
	{
		public PushyLogType Type;
		public string Text = "";
		public DateTime CreatedDateTime = DateTime.Now;
		public string Created = "";

		public PushyLog(PushyLogType type, string text)
		{
			this.Type = type;
			this.Text = text;
			this.Created = string.Format("{0}:{1}", DateTime.Now.Hour, DateTime.Now.Minute);
		}
	}
}
