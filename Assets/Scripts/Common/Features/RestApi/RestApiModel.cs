using System.Net.Http;
using Scripts.Common.Features.Config;

namespace Scripts.Common.Features.RestApi
{
    public class RestApiModel
    {
        public HttpClient Client { get; set; }
        public APIConfig APIConfig { get; set; }
    }

    public class Request
    {
        public string requestedPath;
        public string fileBase64;
    }

    public class MinimapData
    {

    }
}
