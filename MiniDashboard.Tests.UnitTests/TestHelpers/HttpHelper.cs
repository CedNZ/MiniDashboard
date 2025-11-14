using System.Net;
using NSubstitute;

namespace MiniDashboard.Tests.UnitTests.TestHelpers
{
    public static class HttpHelper
    {
        public static IHttpClientFactory GetHttpClientFactory(List<SampleRequest> requests)
        {
            var mockFactory = Substitute.For<IHttpClientFactory>();
            var handler = new TestHttpMessageHandler(requests);
            var httpClient = new HttpClient(handler);
            httpClient.BaseAddress = new Uri("http://localhost/");

            mockFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

            return mockFactory;
        }
    }

    public class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly List<SampleRequest> _requests;

        public TestHttpMessageHandler(List<SampleRequest> requests)
        {
            _requests = requests;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var sampleRequest = _requests.FirstOrDefault(x =>
                x.Method == request.Method && x.Url == request.RequestUri?.LocalPath);

            var response = new HttpResponseMessage(HttpStatusCode.NotFound);

            if (sampleRequest != null)
            {
                response = new HttpResponseMessage(sampleRequest.ResponseCode);
                response.Content = sampleRequest.ResponseContent;
            }

            return response;
        }
    }

    public class SampleRequest
    {
        public HttpMethod Method { get; set; } = HttpMethod.Get;
        public string Url { get; set; } = "";
        public HttpStatusCode ResponseCode { get; set; } = HttpStatusCode.OK;
        public HttpContent? ResponseContent { get; set; }
    }
}
