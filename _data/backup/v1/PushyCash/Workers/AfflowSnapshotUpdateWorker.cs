using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Workers
{
	internal class AfflowSnapshotUpdateWorker : WorkerBase
	{
		public AfflowSnapshotUpdateWorker() : base("afflow", "snapshot", 3 * 60 * 60 * 1000)
		{
		}

		public override void OnLoop()
		{
			PLogger.Debug("Refreshed snapshot data");
			PushyContext.SnapshotManager.Update();
			PushyContext.LinkManager.MapDataWithSnapshot();
		}
	}
}
