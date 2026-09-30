using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LTW2.Data;
using LTW2.Models.DTO;
using LTW2.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace LTW2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthorsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IAuthorRepository _authorRepository;

        public AuthorsController(AppDbContext dbContext, IAuthorRepository authorRepository)
        {
            _dbContext = dbContext;
            _authorRepository = authorRepository;
        }

        [HttpGet("get-all-author")]
        public IActionResult GetAllAuthor([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy, [FromQuery] bool isAscending = true,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            var allAuthors = _authorRepository.GellAllAuthors(filterOn, filterQuery, sortBy, isAscending, pageNumber, pageSize);
            return Ok(allAuthors);
        }

        [HttpGet("get-author-by-id/{id}")]
        public IActionResult GetAuthorById(int id)
        {
            var authorWithId = _authorRepository.GetAuthorById(id);
            return Ok(authorWithId);
        }

        [HttpPost("add-author")]
        public IActionResult AddAuthors([FromBody] AddAuthorRequestDTO addAuthorRequestDTO)
        {
            var authorAdd = _authorRepository.AddAuthor(addAuthorRequestDTO);
            return Ok();
        }

        [HttpPut("update-author-by-id/{id}")]
        public IActionResult UpdateBookById(int id, [FromBody] AuthorNoIdDTO authorDTO)
        {
            var authorUpdate = _authorRepository.UpdateAuthorById(id, authorDTO);
            return Ok(authorUpdate);
        }

        [HttpDelete("delete-author-by-id/{id}")]
        public IActionResult DeleteBookById(int id)
        {
            if (_dbContext.Books_Authors.Any(ba => ba.AuthorId == id))
            {
                return BadRequest(new { message = "Hãy gỡ liên kết trong Book_Author trước khi xóa." });
            }
            var authorDelete = _authorRepository.DeleteAuthorById(id);
            return Ok();
        }
        [HttpGet("{id}/books")]
        public IActionResult GetAuthorWithBooks(int id)
        {
            var authorData = _authorRepository.GetAuthorWithBooks(id);
            if (authorData == null)
            {
                return NotFound();
            }
            return Ok(authorData);
        }
    }
}