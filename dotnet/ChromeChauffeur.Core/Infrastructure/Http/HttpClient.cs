using System.IO;
using System.Net;

namespace ChromeChauffeur.Core.Infrastructure.Http
{
    internal class HttpClient
    {
        public static HttpResponse Get(string url)
        {
            var req = (HttpWebRequest)WebRequest.Create(url);

            try
            {
                using (var response = (HttpWebResponse)req.GetResponse())
                using (var responseStream = response.GetResponseStream())
                using (var sr = new StreamReader(responseStream))
                {
                    var body = sr.ReadToEnd();
                    
                    return new HttpResponse(url, (int)response.StatusCode, body, response.StatusDescription);
                }
            }
            catch (WebException e)
            {
                var response = (HttpWebResponse)e.Response;
                return new HttpResponse(url, (int)response.StatusCode, response.StatusDescription, response.StatusDescription);
            }
        }
    }
}