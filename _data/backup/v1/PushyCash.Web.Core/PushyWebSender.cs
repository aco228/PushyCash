using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PushyCash.Web.Core
{
	public static class PushyWebSender
	{
		private static string SERVER_URL = "http://pushycash.mobilepaywall.com/";

		public static void Send(string action, WebModelBase model)
		{
			Send<WebModelBase>(action, model);
		}

		public static void Send<T>(string action, T data)
		{
			return;
			new Thread(() =>
			{
				try
				{
					var webRequest = System.Net.WebRequest.Create(SERVER_URL + "/DeamonCallback/" + action);
					if (webRequest == null)
						return;

					webRequest.Method = "POST";
					webRequest.Timeout = 12000;
					webRequest.ContentType = "application/json";
					webRequest.ContentType = "application/x-www-form-urlencoded";

					byte[] bytesForSendin = PushyWebDataConverter.ObjToBytes(data);
					webRequest.ContentLength = bytesForSendin.Length;
					Stream newStream = webRequest.GetRequestStream();
					newStream.Write(bytesForSendin, 0, bytesForSendin.Length);

					using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
					using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
					{
						return;
					}
				}
				catch (Exception ex)
				{
					return;
				}
			}).Start();
		}

		public static void CallBackupDeamon()
		{
			new Thread(() =>
			{
				try
				{
					var webRequest = System.Net.WebRequest.Create("http://backup.pushycash.mobilepaywall.com/api/deamon/call");
					if (webRequest == null)
						return;

					webRequest.Method = "GET";
					webRequest.Timeout = 12000;
					webRequest.ContentType = "application/json";
					webRequest.ContentType = "application/x-www-form-urlencoded";
					
					using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
					using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
					{
						return;
					}
				}
				catch (Exception ex)
				{
					return;
				}
			}).Start();
		}

	}
}
