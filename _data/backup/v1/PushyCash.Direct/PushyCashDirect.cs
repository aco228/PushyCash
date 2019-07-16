using Direct.Core.DatabaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Direct
{
	public class PushyCashDirect : DirectDatabaseMsSql
	{
		private static PushyCashDirect _instance = null;

		public PushyCashDirect() : base("PushyCash", "core")
		{
			string connectionString = "";
			try
			{
				 connectionString = System.Configuration.ConfigurationManager.
					ConnectionStrings[this.DatabaseName].ConnectionString;
			}
			catch (Exception e)
			{
				connectionString = "Data Source=192.168.11.104;Initial Catalog=PushyCash;uid=sa;pwd=m_q-6dGyRwcTf+b;Max Pool Size=1000";
			}

			this.SetConnectionString(connectionString);
		}

		public static PushyCashDirect Instance
		{
			get
			{
				if (_instance != null)
					return _instance;

				_instance = new PushyCashDirect();
				return _instance;
			}
		}
	}
}
