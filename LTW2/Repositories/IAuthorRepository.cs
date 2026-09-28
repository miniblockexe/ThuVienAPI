using LTW2.Models.Domain;
using LTW2.Models.DTO;

namespace LTW2.Repositories
{
    public interface IAuthorRepository
    {
        List<AuthorDTO> GellAllAuthors(string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000);
        AuthorNoIdDTO GetAuthorById(int id);
        AddAuthorRequestDTO AddAuthor(AddAuthorRequestDTO addAuthorRequestDTO);
        AuthorNoIdDTO UpdateAuthorById(int id, AuthorNoIdDTO authorNoIdDTO);
        Author? DeleteAuthorById(int id);
        AuthorWithBooksDTO GetAuthorWithBooks(int id);
    }
}