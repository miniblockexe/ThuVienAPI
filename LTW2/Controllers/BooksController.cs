using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using LTW2.Data;
using LTW2.Models.Domain;
using LTW2.Models.DTO;
using LTW2.Repositories;
using LTW2.CustomActionFilter;
using Microsoft.AspNetCore.Authorization;

namespace LTW2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IBookRepository _bookRepository;

        public BooksController(AppDbContext dbContext, IBookRepository bookRepository)
        {
            _dbContext = dbContext;
            _bookRepository = bookRepository;
        }

        [HttpGet("get-all-books")]
        public IActionResult GetAll()
        {
            // su dung reposity pattern
            var allBooks = _bookRepository.GetAllBooks();
            return Ok(allBooks);
        }

        [HttpGet]
        [Route("get-book-by-id/{id}")]
        public IActionResult GetBookById([FromRoute] int id)
        {
            var bookWithIdDTO = _bookRepository.GetBookById(id);
            return Ok(bookWithIdDTO);
        }

        [HttpPost("add-book")]
        [ValidateModel]
        // [Authorize(Roles = "Write")]
        public IActionResult AddBook([FromBody] AddBookRequestDTO addBookRequestDTO)
        {
            if (ValidateAddBook(addBookRequestDTO))
            {
                var bookAdd = _bookRepository.AddBook(addBookRequestDTO);
                return Ok(bookAdd);
            }
            return BadRequest(ModelState);
        }

        [HttpPut("update-book-by-id/{id}")]
        public IActionResult UpdateBookById(int id, [FromBody] AddBookRequestDTO bookDTO)
        {
            var updateBook = _bookRepository.UpdateBookById(id, bookDTO);
            return Ok(updateBook);
        }

        [HttpDelete("delete-book-by-id/{id}")]
        public IActionResult DeleteBookById(int id)
        {
            var deleteBook = _bookRepository.DeleteBookById(id);
            return Ok(deleteBook);
        }
        #region Private methods
        private bool ValidateAddBook(AddBookRequestDTO addBookRequestDTO)
        {
            if (addBookRequestDTO == null)
            {
                ModelState.AddModelError(nameof(addBookRequestDTO), $"Please add book data");
                return false;
            }

            if (!_dbContext.Publishers.Any(p => p.Id == addBookRequestDTO.PublisherID))
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.PublisherID),
                    $"Publisher ID {addBookRequestDTO.PublisherID} does not exist.");
            }

            foreach (var authorId in addBookRequestDTO.AuthorIds)
            {
                if (!_dbContext.Authors.Any(a => a.Id == authorId))
                {
                    ModelState.AddModelError(nameof(addBookRequestDTO.AuthorIds),
                        $"Author ID {authorId} does not exist.");
                }
            }

            if (addBookRequestDTO.AuthorIds == null || !addBookRequestDTO.AuthorIds.Any())
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.AuthorIds), "Mỗi sách phải có ít nhất một tác giả.");
            }
            else
            {
                foreach (var authorId in addBookRequestDTO.AuthorIds)
                {
                    if (!_dbContext.Authors.Any(a => a.Id == authorId))
                    {
                        ModelState.AddModelError(nameof(addBookRequestDTO.AuthorIds),
                            $"Author ID {authorId} does not exist.");
                        continue;
                    }

                    int currentBookCount = _dbContext.Books_Authors.Count(ba => ba.AuthorId == authorId);
                    if (currentBookCount >= 20)
                    {
                        ModelState.AddModelError(nameof(addBookRequestDTO.AuthorIds), $"Tác giả (ID: {authorId}) đã đạt giới hạn tối đa 20 cuốn sách.");
                    }
                }

                var duplicateAuthors = addBookRequestDTO.AuthorIds.GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                if (duplicateAuthors.Any())
                {
                    ModelState.AddModelError(nameof(addBookRequestDTO.AuthorIds), "Không được phép gán trùng một tác giả cho cùng một sách.");
                }
            }

            int targetYear = addBookRequestDTO.DateAdded.Year;
            int publisherBookCountInYear = _dbContext.Books.Count(b => b.PublisherID == addBookRequestDTO.PublisherID && b.DateAdded.Year == targetYear);
            if (publisherBookCountInYear >= 100)
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.PublisherID), $"Nhà xuất bản này đã xuất bản quá 100 cuốn sách trong năm {targetYear}.");
            }

            bool isDuplicateTitleInPublisher = _dbContext.Books.Any(b => b.PublisherID == addBookRequestDTO.PublisherID && b.Title == addBookRequestDTO.Title);
            if (isDuplicateTitleInPublisher)
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.Title), "Tiêu đề sách đã tồn tại ở nhà xuất bản này.");
            }
            // kiem tra Description NotNull
            if (string.IsNullOrEmpty(addBookRequestDTO.Description))
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.Description),
                $"{nameof(addBookRequestDTO.Description)} cannot be null");
            }

            // kiem tra rating (0,5)
            if (addBookRequestDTO.Rate < 0 || addBookRequestDTO.Rate > 5)
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.Rate),
                $"{nameof(addBookRequestDTO.Rate)} cannot be less than 0 and more than 5");
            }

            if (ModelState.ErrorCount > 0)
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}