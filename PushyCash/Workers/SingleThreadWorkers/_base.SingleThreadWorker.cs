using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Workers.SingleThreadWorkers
{
	public class SingleThreadWorker : WorkerBase
	{
		public List<SingleThreadOperationBase> Operations { get; protected set; } = null;

		public SingleThreadWorker() : base("single_thread", "single_thread", 1000)
		{
			this.Operations = new List<SingleThreadOperationBase>();
			this.Operations.Add(new GetCostsAndPayouts());
			this.Operations.Add(new SlackReporter());
			this.Operations.Add(new LogDeletion());
		}

		public override void OnLoop()
		{
			foreach (var o in this.Operations)
				if (o.ShouldBeCalled)
					o.Call();
		}
	}
}
