using Direct.Core;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Direct.Models.Main
{
	public class DirectMainConfiguration : PushyDirectModelBase
	{
		public DirectMainConfiguration() : base("MainConfiguration") { }

		public int MainConfigurationID { get; set; } = -1;
		public double DailiyBudget { get; set; } = 0.0;

		public DirectMainConfiguration Load()
		{
			return PushyCashDirect.Instance.LoadContainer("SELECT TOP 1 * FROM PushyCash.core.MainConfiguration").Convert<DirectMainConfiguration>();
		}
	}
}
