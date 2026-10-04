using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Library_web.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Library_web.Controllers
{
    public class PublishersController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly string baseUrl;

        public PublishersController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            this.httpClientFactory = httpClientFactory;
            baseUrl = config["Api:BaseUrl"];
        }

        // Liệt kê tất cả
        public async Task<IActionResult> Index()
        {
            List<publisherDTO> response = new List<publisherDTO>();
            try
            {
                var client = httpClientFactory.CreateClient();
                var httpResponseMess = await client.GetAsync(baseUrl + "/api/Publishers/get-all-publisher");
                httpResponseMess.EnsureSuccessStatusCode();
                response.AddRange(await httpResponseMess.Content.ReadFromJsonAsync<IEnumerable<publisherDTO>>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
            return View(response);
        }

        // Liệt kê theo Id
        public async Task<IActionResult> listPublisher(int id)
        {
            publisherDTO response = new publisherDTO();
            try
            {
                var client = httpClientFactory.CreateClient();
                var httpResponseMess = await client.GetAsync(baseUrl + "/api/Publishers/get-publisher-by-id/" + id);
                httpResponseMess.EnsureSuccessStatusCode();
                var data = await httpResponseMess.Content.ReadFromJsonAsync<publisherNoIdDTO>();
                response = new publisherDTO { Id = id, Name = data?.Name ?? "" };
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(response);
        }

        // Thêm
        [HttpGet]
        public IActionResult addPublisher()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> addPublisher(publisherNoIdDTO dto)
        {
            try
            {
                var client = httpClientFactory.CreateClient();
                var httpRequestMess = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(baseUrl + "/api/Publishers/add-publisher"),
                    Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8,
                        MediaTypeNames.Application.Json)
                };
                var httpResponseMess = await client.SendAsync(httpRequestMess);
                if (httpResponseMess.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Publishers");
                }
                ViewBag.Error = await httpResponseMess.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(dto);
        }

        // Sửa
        [HttpGet]
        public async Task<IActionResult> editPublisher(int id)
        {
            publisherDTO response = new publisherDTO();
            try
            {
                var client = httpClientFactory.CreateClient();
                var httpResponseMess = await client.GetAsync(baseUrl + "/api/Publishers/get-publisher-by-id/" + id);
                httpResponseMess.EnsureSuccessStatusCode();
                var data = await httpResponseMess.Content.ReadFromJsonAsync<publisherNoIdDTO>();
                response = new publisherDTO { Id = id, Name = data?.Name ?? "" };
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> editPublisher([FromRoute] int id, publisherNoIdDTO dto)
        {
            try
            {
                var client = httpClientFactory.CreateClient();
                var httpRequestMess = new HttpRequestMessage()
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri(baseUrl + "/api/Publishers/update-publisher-by-id/" + id),
                    Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8,
                        MediaTypeNames.Application.Json)
                };
                var httpResponseMess = await client.SendAsync(httpRequestMess);
                if (httpResponseMess.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Publishers");
                }
                ViewBag.Error = await httpResponseMess.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(new publisherDTO { Id = id, Name = dto.Name });
        }

        // Xóa
        [HttpGet]
        public async Task<IActionResult> delPublisher([FromRoute] int id)
        {
            try
            {
                var client = httpClientFactory.CreateClient();
                var httpResponseMess = await client.DeleteAsync(baseUrl + "/api/Publishers/delete-publisher-by-id/" + id);
                if (!httpResponseMess.IsSuccessStatusCode)
                {
                    TempData["Error"] = await httpResponseMess.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index", "Publishers");
        }
    }
}
