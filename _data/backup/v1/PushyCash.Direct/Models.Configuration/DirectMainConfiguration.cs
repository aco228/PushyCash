using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Direct.Models.Configuration
{
	public class DirectMainConfiguration : DirectModel
	{
		public DirectMainConfiguration() : base("MainConfiguration") { }

		public int MainConfigurationID { get; set; } = -1;
		public double DailyBudget { get; set; } = 0.0;


		public static DirectMainConfiguration Load()
		{
			return PushyCashDirect.Instance.LoadContainer("SELECT TOP 1 * FROM PushyCash.core.MainConfiguration").Convert<DirectMainConfiguration>();
		}

	}
}
