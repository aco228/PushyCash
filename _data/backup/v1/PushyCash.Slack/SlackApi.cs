using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Slack
{
	internal class SlackApi
	{
		private static DateTime LastSending = DateTime.Now;

		public string Send(string url, string text)
		{
			if ((DateTime.Now - LastSending).TotalSeconds <= 1)
				System.Threading.Thread.Sleep(1000);

			try
			{
				var webRequest = System.Net.WebRequest.Create(url);
				if (webRequest == null)
					return null;

				webRequest.Method = "POST";
				webRequest.Timeout = 12000;
				webRequest.ContentType = "application/json";
				//webRequest.Headers.Add("Content-type", "application/json");

				string postData = "{\"text\":\"input\"}".Replace("input", text);
				webRequest.ContentType = "application/json";
				ASCIIEncoding encoding = new ASCIIEncoding();
				byte[] byte1 = encoding.GetBytes(postData);
				webRequest.ContentLength = byte1.Length;
				Stream newStream = webRequest.GetRequestStream();
				newStream.Write(byte1, 0, byte1.Length);

				using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
				{
					using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
					{
						var jsonResponse = sr.ReadToEnd();
						return jsonResponse;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return null;
			}
		}
	}
}
