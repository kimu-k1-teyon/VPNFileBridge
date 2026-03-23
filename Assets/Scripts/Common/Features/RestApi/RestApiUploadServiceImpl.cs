using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using VContainer;

namespace Scripts.Common.Features.RestApi
{
    public class RestApiUploadServiceImpl : IRestApiUploadService
    {
        [Inject] IRestApiService _service;
        [Inject] RestApiModel _model;

        public async Task<HttpResponseMessage> UploadAsync(
            string uploadId,
            string filePath,
            CancellationToken cancellationToken)
        {
            using var fileStream = File.OpenRead(filePath);
            using var request = CreateRequest(uploadId, filePath, fileStream);
            return await _service.SendAsync(request, cancellationToken, _model.UploadTimeoutMS);
        }

        HttpRequestMessage CreateRequest(string uploadId, string filePath, Stream fileStream)
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(uploadId), "uploadId");

            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            content.Add(fileContent, "file", Path.GetFileName(filePath));

            var request = new HttpRequestMessage(HttpMethod.Post, _model.UploadEndpoint)
            {
                Content = content
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return request;
        }
    }
}
