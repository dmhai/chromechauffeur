namespace ChromeChauffeur.Core.Infrastructure.Http
{
    internal class HttpResponse
    {
        public HttpResponse(string requestedUrl, int statusCode, string body, string statusDescription)
        {
            StatusCode = statusCode;
            Body = body;
            StatusDescription = statusDescription;
            RequestedUrl = requestedUrl;
        }

        public readonly string RequestedUrl;
        public readonly int StatusCode;
        public readonly string StatusDescription;
        public readonly string Body;
    }
}