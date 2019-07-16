using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Workers
{
	internal class SnapshotUpdateWorker : WorkerBase
	{
		public SnapshotUpdateWorker() : base(15 * 60 * 1000)
		{
		}

		public override void OnLoop()
		{
			PLogger.Debug("Refreshed snapshot data");
			PushyContext.SnapshotManager.Update();
		}
	}
}
