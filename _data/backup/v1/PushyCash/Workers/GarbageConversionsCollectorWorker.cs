using PushyCash.Links;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Workers
{
	public class GarbageConversionsCollectorWorker : WorkerBase
	{
		public GarbageConversionsCollectorWorker() : base(1000)
		{
		}

		public override void OnLoop()
		{
			foreach (KeyValuePair<string, List<Link>> dict in PushyContext.LinkManager.CountryVerticalLinkMapCache)
				foreach (Link link in dict.Value)
					link.OnGarbageCollection();
			PushyContext.SnapshotManager.OnGarbageCollector();
		}
	}
}
