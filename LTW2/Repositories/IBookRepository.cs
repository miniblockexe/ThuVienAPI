using LTW2.Models.Domain;
using LTW2.Models.DTO;

namespace LTW2.Repositories
{
    public interface IBookRepository
    {
        List<BookWithAuthorAndPublisherDTO> GetAllBooks(string? filterOn = null, string? filterQuery = null,
    string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000);
        BookWithAuthorAndPublisherDTO GetBookById(int id);
        AddBookRequestDTO AddBook(AddBookRequestDTO addBookRequestDTO);
        AddBookRequestDTO? UpdateBookById(int id, AddBookRequestDTO bookDTO);
        Book? DeleteBookById(int id);
    }
}