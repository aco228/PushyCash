using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Direct
{
	public abstract class PushyDirectModelBase : DirectModel
	{
		public PushyDirectModelBase(string tableName) : base(PushyCashDirect.Instance, tableName) { }
	}
}
