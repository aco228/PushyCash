using Newtonsoft.Json;
using PushyCash;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCashHelper
{
	class Program
	{
		static void Main(string[] args)
		{
			PushyContext.AfflowManager = new PushyCash.Afflow.AfflowManager();
			

			string text = File.ReadAllText(@"D:\Projects\AkoProjects\dot.net\PushyCash\_data\networks\propellerads\mobileOperators.json");
			dynamic json = JsonConvert.DeserializeObject(text);
			string result = "";
			string nonfound = "";

			foreach (dynamic row in json)
			{
				string mno = row.title.ToString().Split('(')[0].Trim();
				string response = PushyContext.AfflowManager.MobileOperatorMap.GetNameDeepSearch(row.parent.ToString(), mno);

				if (string.IsNullOrEmpty(response))
				{
					nonfound += $"{row.parent} {row.title}" + Environment.NewLine;
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"{row.parent} {row.title}");
				}
				else
				{
					result += $"{row.parent.ToString().ToUpper()}#{response}#{row.id.ToString()}" + Environment.NewLine;
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine($"                {row.parent} {response} ### {row.title}");
				}

				
			}

			Console.ReadKey();
		}
	}
}
