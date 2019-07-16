using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Workers
{
	public class GarbageCollectorSnapshotOfferWorker : WorkerBase
	{
		public GarbageCollectorSnapshotOfferWorker() : base("gc", "snapshot", 5000)
		{
		}

		public override void OnLoop()
		{
			PushyContext.SnapshotManager.OnGarbageCollector();
		}
	}
}
