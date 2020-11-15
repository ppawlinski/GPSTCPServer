using System;
using System.IO;
using System.Net;

namespace GPSTCPServer
{
    public static class GetRequest
    {
        public static async System.Threading.Tasks.Task<string> GetFromURLAsync(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Headers.Add(HttpRequestHeader.UserAgent, "TCPServerProject v1.0");
            try
            {
                var webResponse = await request.GetResponseAsync();
                var webStream = webResponse.GetResponseStream();
                var responseReader = new StreamReader(webStream);
                return responseReader.ReadToEnd();
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception) {  }
            
            
            return "";
        }
    }
}
