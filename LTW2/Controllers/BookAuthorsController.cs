using LTW2.Data;
using LTW2.Models.Domain;
using LTW2.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace LTW2.Controllers
{
    [Route("api/book-authors")]
    [ApiController]
    public class BookAuthorsController : ControllerBase
    {
        private const int MaxBooksPerAuthor = 20; // Bài 10

        private readonly AppDbContext _dbContext;

        public BookAuthorsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public IActionResult AddBookAuthor([FromBody] AddBookAuthorRequestDTO dto)
        {
            // Bài 5
            if (!_dbContext.Books.Any(b => b.Id == dto.BookId))
            {
                ModelState.AddModelError(nameof(dto.BookId), $"Book ID {dto.BookId} không tồn tại.");
            }
            if (!_dbContext.Authors.Any(a => a.Id == dto.AuthorId))
            {
                ModelState.AddModelError(nameof(dto.AuthorId), $"Author ID {dto.AuthorId} không tồn tại.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Bài 6 + Bài 14
            bool exists = _dbContext.Books_Authors
                .Any(ba => ba.BookId == dto.BookId && ba.AuthorId == dto.AuthorId);
            if (exists)
            {
                return Conflict(new { message = $"Tác giả {dto.AuthorId} đã được gán cho sách {dto.BookId}." });
            }

            // Bài 10
            int currentBookCount = _dbContext.Books_Authors.Count(ba => ba.AuthorId == dto.AuthorId);
            if (currentBookCount >= MaxBooksPerAuthor)
            {
                ModelState.AddModelError(nameof(dto.AuthorId),
                    $"Tác giả (ID: {dto.AuthorId}) đã đạt giới hạn tối đa {MaxBooksPerAuthor} cuốn sách.");
                return BadRequest(ModelState);
            }

            var link = new Book_Author
            {
                BookId = dto.BookId,
                AuthorId = dto.AuthorId
            };
            _dbContext.Books_Authors.Add(link);
            _dbContext.SaveChanges();

            return StatusCode(StatusCodes.Status201Created, new { link.Id, link.BookId, link.AuthorId });
        }

        // Bài 15
        [HttpDelete]
        public IActionResult RemoveBookAuthor([FromQuery] int bookId, [FromQuery] int authorId)
        {
            var link = _dbContext.Books_Authors
                .FirstOrDefault(ba => ba.BookId == bookId && ba.AuthorId == authorId);
            if (link == null)
            {
                return NotFound(new { message = "Không tìm thấy liên kết Book–Author này." });
            }

            // Bài 9
            if (_dbContext.Books_Authors.Count(ba => ba.BookId == bookId) <= 1)
            {
                return BadRequest(new { message = "Không thể gỡ tác giả cuối cùng: mỗi sách phải có ít nhất 1 tác giả." });
            }

            _dbContext.Books_Authors.Remove(link);
            _dbContext.SaveChanges();
            return Ok(new { message = "Đã gỡ liên kết." });
        }
    }
}