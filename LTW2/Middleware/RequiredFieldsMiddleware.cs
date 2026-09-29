namespace LTW2.Middleware
{
    public class RequiredFieldsMiddleware
    {
        private readonly RequestDelegate _next;

        private static readonly Dictionary<string, string[]> _requiredFields = new()
        {
            { "/api/books",        new[] { "title", "publisherid" } },
            { "/api/book-authors", new[] { "bookid", "authorid" } },
            { "/api/authors",      new[] { "fullname" } },
            { "/api/publishers",   new[] { "name" } },
        };

        public RequiredFieldsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Method == HttpMethods.Post ||
                context.Request.Method == HttpMethods.Put)
            {
                string path = context.Request.Path.Value?.ToLower() ?? "";

                var matchedRoute = _requiredFields.Keys
                    .FirstOrDefault(r => path.StartsWith(r));

                if (matchedRoute != null)
                {
                    context.Request.EnableBuffering();

                    using var reader = new StreamReader(
                        context.Request.Body,
                        leaveOpen: true);

                    string body = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;

                    if (string.IsNullOrWhiteSpace(body))
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            message = "Request body không được để trống."
                        });
                        return;
                    }

                    System.Text.Json.JsonDocument? doc = null;
                    try
                    {
                        doc = System.Text.Json.JsonDocument.Parse(body);
                    }
                    catch
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            message = "Request body không phải JSON hợp lệ."
                        });
                        return;
                    }

                    var missing = new List<string>();
                    foreach (var field in _requiredFields[matchedRoute])
                    {
                        bool found = doc.RootElement
                            .EnumerateObject()
                            .Any(p => p.Name.ToLower() == field &&
                                      p.Value.ValueKind != System.Text.Json.JsonValueKind.Null &&
                                      p.Value.ToString() != "");
                        if (!found) missing.Add(field);
                    }

                    if (missing.Any())
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            message = $"Thiếu trường bắt buộc: {string.Join(", ", missing)}"
                        });
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}