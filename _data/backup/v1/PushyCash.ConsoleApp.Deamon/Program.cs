using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.ConsoleApp.Deamon
{
	class Program
	{
		static void Main(string[] args)
		{
			PushyContext.OnEntireLoadProcessIsFinished += OnLoadFinished;
			PushyContext.Init();
			Console.ReadKey();
		}

		static void OnLoadFinished()
		{
			PushyContext.StartCollecting();
		}
	}
}
