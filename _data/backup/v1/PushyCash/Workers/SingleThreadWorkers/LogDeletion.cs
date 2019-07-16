using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Workers.SingleThreadWorkers
{
	public class LogDeletion : SingleThreadOperationBase
	{
		public LogDeletion() : base(delayInSeconds:1, executeImmediately:false)
		{
		}

		protected override void OnCall()
		{
			PLogger.PreformCleanup();
		}
	}
}
