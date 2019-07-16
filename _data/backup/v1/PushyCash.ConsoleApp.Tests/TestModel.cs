using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.ConsoleApp.Tests
{
	public class TestModel : DirectModel
	{
		public TestModel() : base("TestTable") { }

		public int ID { get; set; } = -1;
		public bool IsTrue { get; set; } = false;
		public string Name { get; set; } = "";
		public double Payout { get; set; } = 0.0;
		public int? Nula { get; set; } = null;
		public DateTime Date { get; set; } = DateTime.Now;
	}
}
