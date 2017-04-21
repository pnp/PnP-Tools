using System;
using System.IO;
using System.Net;

namespace TIP.WebJob
{
	internal class HttpHelper
	{
		//public static async Task<bool> PostJsonMessage(string url, string body)
		internal static bool PostJsonMessage(string url, string body)
		{
			string responseContent = String.Empty;


			var request = (HttpWebRequest)HttpWebRequest.Create(url);

			request.Method = "POST";
			request.Accept = "application/json";
			request.ContentType = "application/json";

			using (var writer = new StreamWriter(request.GetRequestStream()))
			{
				writer.Write(body);
			}

			var response = (HttpWebResponse)request.GetResponse();
			using (var reader = new StreamReader(response.GetResponseStream()))
			{
				responseContent = reader.ReadToEnd();
			}

			return (response.StatusCode == HttpStatusCode.OK);

		}
	}
}
