using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Afflow.API
{
	internal class AfflowApiBase
	{
		protected AfflowManager _manager = null;
		protected static string TOKEN = "4cb20a96a3ac100deab3bf486c06a2bedb3a3beb";
		private static readonly int AVALIABLE_RETRIES = 15;
		public static readonly int TIMEOUT_MILISECONDS = 2000;

		public AfflowApiBase(AfflowManager manager)
		{
			this._manager = manager;
		}

		protected dynamic Request(string requestUrl, bool tolerateNull = false)
		{
			int retries = 0;
			for(; ; )
			{
				if(retries >= AVALIABLE_RETRIES)
				{
					_manager.OnServerFatal?.Invoke("afflow", "Afflow fatal on get link: " + requestUrl);
					return null;
				}

				try
				{
					var webRequest = System.Net.WebRequest.Create(requestUrl);
					if (webRequest == null)
						return null;

					webRequest.Method = "GET";
					webRequest.Timeout = 1500;
					webRequest.ContentType = "application/json";
					webRequest.Headers.Add("x_afflow_api_token", TOKEN);

					using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
					{
						using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
						{
							var jsonResponse = sr.ReadToEnd();
							dynamic json = JsonConvert.DeserializeObject(jsonResponse);
							if (json == null || json.ToString().Equals("[]"))
								return null;

							return json;
						}
					}
				}
				catch (Exception ex)
				{
					if (tolerateNull)
						return null;
					retries++;
					System.Threading.Thread.Sleep(TIMEOUT_MILISECONDS);
				}
			}
		}

		protected dynamic Post(string requestUrl, string data)
		{
			int retries = 0;
			for (; ; )
			{
				if (retries >= AVALIABLE_RETRIES)
				{
					_manager.OnServerFatal?.Invoke("afflow", "Afflow fatal on get link: " + requestUrl);
					return null;
				}

				try
				{
					var webRequest = System.Net.WebRequest.Create(requestUrl);
					if (webRequest == null)
						return null;

					webRequest.Method = "POST";
					webRequest.Timeout = 12000;
					webRequest.ContentType = "application/json";
					webRequest.Headers.Add("X_AFFLOW_API_TOKEN", TOKEN);
					webRequest.ContentType = "application/x-www-form-urlencoded";

					string postData = data;
					ASCIIEncoding encoding = new ASCIIEncoding();
					byte[] byte1 = encoding.GetBytes(postData);
					webRequest.ContentLength = byte1.Length;
					Stream newStream = webRequest.GetRequestStream();
					newStream.Write(byte1, 0, byte1.Length);

					using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
					using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
					{
						var jsonResponse = sr.ReadToEnd();
						dynamic json = JsonConvert.DeserializeObject(jsonResponse);

						//Console.WriteLine(String.Format("Response: {0}", jsonResponse));
						return json;
					}
				}
				catch (Exception ex)
				{
					retries++;
					System.Threading.Thread.Sleep(TIMEOUT_MILISECONDS);
				}
			}
		}

		protected long DateToUnix(DateTime foo)
		{
			return ((DateTimeOffset)foo).ToUnixTimeSeconds();
		}

	}
}
