using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.TrafficNetworks.Core
{
	public abstract class TrafficSourceAfflowMapBase
	{
		public virtual int NID { get; protected set; } = -1;
		public virtual string DefaultOptionsData { get; protected set; } = string.Empty;
	}
}
