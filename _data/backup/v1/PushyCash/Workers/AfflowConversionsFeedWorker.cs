using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Workers
{
	public class AfflowConversionsFeedWorker : WorkerBase
	{
		public AfflowConversionsFeedWorker() : base("afflow", "conversions", 2000)
		{
		}

		public override void OnLoop()
		{
		}
	}
}
