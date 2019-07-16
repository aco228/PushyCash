using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Workers
{
	public class AfflowLoginWorker : WorkerBase
	{
		public AfflowLoginWorker() : base("afflow", "login", 30 * 60 * 1000)
		{
		}

		public override void OnLoop()
		{
			PLogger.Debug("Preformed login into afflow");
			PushyContext.AfflowManager.PreformLogin();
		}
	}
}
