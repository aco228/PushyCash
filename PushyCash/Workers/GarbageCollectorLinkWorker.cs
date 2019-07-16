using PushyCash.Links;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Workers
{
	public class GarbageCollectorLinkWorker : WorkerBase
	{
		public GarbageCollectorLinkWorker() : base("gc", "links", 30 * 1000)
		{
		}

		public override void OnLoop()
		{
			DateTime dateTime = DateTime.Now;
			try
			{
				foreach (KeyValuePair<string, List<Link>> dict in PushyContext.LinkManager.CountryVerticalLinkMapCache)
					foreach (Link link in dict.Value)
						link.OnGarbageCollection();
			}
			catch (Exception e)
			{
				PLogger.Error("GCLink fatal: " + e.ToString());
				int a = 0;
			}
		}
	}
}
