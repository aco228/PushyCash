using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Afflow.API
{
	public class AfflowTimezones
	{
		public string City { get; set; } = "";

		public string TZParam
		{
			get
			{
				switch(this.City)
				{
					case "London": return "Europe/London";
					default:
						return "Europe/Belgrade";
				}
			}
		}
		
	}
}
