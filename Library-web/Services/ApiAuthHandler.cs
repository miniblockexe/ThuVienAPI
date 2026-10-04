using System.Net;
using System.Net.Http.Headers;

namespace Library_web.Services
{
    // Lấy JWT từ Session (đã lưu lúc login) và gắn vào mọi request gọi API
    public class ApiAuthHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _http;

        public ApiAuthHandler(IHttpContextAccessor http)
        {
            _http = http;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken ct)
        {
            var session = _http.HttpContext?.Session;
            var token = session?.GetString("jwt");

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, ct);

            // Token hết hạn -> xóa, lần sau bị chuyển về trang login
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                session?.Remove("jwt");
            }
            return response;
        }
    }
}