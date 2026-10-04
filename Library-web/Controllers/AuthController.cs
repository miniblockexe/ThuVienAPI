using Library_web.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Library_web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _factory;
        private readonly IConfiguration _config;

        public AuthController(IHttpClientFactory factory, IConfiguration config)
        {
            _factory = factory;
            _config = config;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(loginDTO dto)
        {
            if (!ModelState.IsValid) return View(dto);

            try
            {
                var client = _factory.CreateClient("ApiLogin");
                var res = await client.PostAsJsonAsync($"{_config["Api:BaseUrl"]}/api/User/Login", dto);

                if (!res.IsSuccessStatusCode)
                {
                    ViewBag.Error = await res.Content.ReadAsStringAsync();
                    return View(dto);
                }

                var data = await res.Content.ReadFromJsonAsync<loginResponseDTO>();
                if (string.IsNullOrEmpty(data?.JwtToken))
                {
                    ViewBag.Error = "API không trả về token";
                    return View(dto);
                }

                HttpContext.Session.SetString("jwt", data.JwtToken);
                return RedirectToAction("Index", "Books");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(dto);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("jwt");
            return RedirectToAction("Login");
        }
    }
}