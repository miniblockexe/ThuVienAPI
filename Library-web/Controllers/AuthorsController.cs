using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Library_web.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Library_web.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly string baseUrl;

        public AuthorsController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            this.httpClientFactory = httpClientFactory;
            baseUrl = config["Api:BaseUrl"];
        }

        // Liệt kê tất cả
        public async Task<IActionResult> Index()
        {
            List<authorDTO> response = new List<authorDTO>();
            try
            {
                var client = httpClientFactory.CreateClient();
                var httpResponseMess = await client.GetAsync(baseUrl + "/api/Authors/get-all-author");
                httpResponseMess.EnsureSuccessStatusCode();
                response.AddRange(await httpResponseMess.Content.ReadFromJsonAsync<IEnumerable<authorDTO>>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
            return View(response);
        }

        // Liệt kê theo Id
        public async Task<IActionResult> listAuthor(int id)
        {
            authorDTO response = new authorDTO();
            try
            {
                var client = httpClientFactory.CreateClient();
                var httpResponseMess = await client.GetAsync(baseUrl + "/api/Authors/get-author-by-id/" + id);
                httpResponseMess.EnsureSuccessStatusCode();
                var data = await httpResponseMess.Content.ReadFromJsonAsync<authorNoIdDTO>();
                response = new authorDTO { Id = id, FullName = data?.FullName ?? "" };
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(response);
        }

        // Thêm
        [HttpGet]
        public IActionResult addAuthor()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> addAuthor(authorNoIdDTO dto)
        {
            try
            {
                var client = httpClientFactory.CreateClient();
                var httpRequestMess = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(baseUrl + "/api/Authors/add-author"),
                    Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8,
                        MediaTypeNames.Application.Json)
                };
                var httpResponseMess = await client.SendAsync(httpRequestMess);
                if (httpResponseMess.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Authors");
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
        public async Task<IActionResult> editAuthor(int id)
        {
            authorDTO response = new authorDTO();
            try
            {
                var client = httpClientFactory.CreateClient();
                var httpResponseMess = await client.GetAsync(baseUrl + "/api/Authors/get-author-by-id/" + id);
                httpResponseMess.EnsureSuccessStatusCode();
                var data = await httpResponseMess.Content.ReadFromJsonAsync<authorNoIdDTO>();
                response = new authorDTO { Id = id, FullName = data?.FullName ?? "" };
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> editAuthor([FromRoute] int id, authorNoIdDTO dto)
        {
            try
            {
                var client = httpClientFactory.CreateClient();
                var httpRequestMess = new HttpRequestMessage()
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri(baseUrl + "/api/Authors/update-author-by-id/" + id),
                    Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8,
                        MediaTypeNames.Application.Json)
                };
                var httpResponseMess = await client.SendAsync(httpRequestMess);
                if (httpResponseMess.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Authors");
                }
                ViewBag.Error = await httpResponseMess.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(new authorDTO { Id = id, FullName = dto.FullName });
        }

        // Xóa
        [HttpGet]
        public async Task<IActionResult> delAuthor([FromRoute] int id)
        {
            try
            {
                var client = httpClientFactory.CreateClient();
                var httpResponseMess = await client.DeleteAsync(baseUrl + "/api/Authors/delete-author-by-id/" + id);
                if (!httpResponseMess.IsSuccessStatusCode)
                {
                    TempData["Error"] = await httpResponseMess.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index", "Authors");
        }
    }
}
