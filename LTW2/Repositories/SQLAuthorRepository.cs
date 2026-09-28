using LTW2.Data;
using LTW2.Models.Domain;
using LTW2.Models.DTO;

namespace LTW2.Repositories
{
    public class SQLAuthorRepository : IAuthorRepository
    {
        private readonly AppDbContext _dbContext;

        public SQLAuthorRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<AuthorDTO> GellAllAuthors(string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
        {
            var allAuthors = _dbContext.Authors.Select(a => new AuthorDTO
            {
                Id = a.Id,
                FullName = a.FullName
            }).AsQueryable();

            // filtering
            if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
            {
                if (filterOn.Equals("fullname", StringComparison.OrdinalIgnoreCase))
                {
                    allAuthors = allAuthors.Where(x => x.FullName.Contains(filterQuery));
                }
            }

            // sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                if (sortBy.Equals("fullname", StringComparison.OrdinalIgnoreCase))
                {
                    allAuthors = isAscending
                        ? allAuthors.OrderBy(x => x.FullName)
                        : allAuthors.OrderByDescending(x => x.FullName);
                }
            }

            // pagination
            var skipResults = (pageNumber - 1) * pageSize;
            return allAuthors.Skip(skipResults).Take(pageSize).ToList();
        }

        public AuthorNoIdDTO GetAuthorById(int id)
        {
            // get book Domain model from Db
            var authorWithIdDomain = _dbContext.Authors.FirstOrDefault(x => x.Id == id);

            if (authorWithIdDomain == null)
            {
                return null;
            }
            //Map Domain Model to DTOs
            var authorNoIdDTO = new AuthorNoIdDTO
            {
                FullName = authorWithIdDomain.FullName,
            };
            return authorNoIdDTO;
        }

        public AddAuthorRequestDTO AddAuthor(AddAuthorRequestDTO addAuthorRequestDTO)
        {
            var authorDomainModel = new Author
            {
                FullName = addAuthorRequestDTO.FullName,
            };
            //Use Domain Model to create Author
            _dbContext.Authors.Add(authorDomainModel);
            _dbContext.SaveChanges();
            return addAuthorRequestDTO;
        }

        public AuthorNoIdDTO UpdateAuthorById(int id, AuthorNoIdDTO authorNoIdDTO)
        {
            var authorDomain = _dbContext.Authors.FirstOrDefault(n => n.Id == id);
            if (authorDomain != null)
            {
                authorDomain.FullName = authorNoIdDTO.FullName;
                _dbContext.SaveChanges();
            }
            return authorNoIdDTO;
        }

        public Author? DeleteAuthorById(int id)
        {
            var authorDomain = _dbContext.Authors.FirstOrDefault(n => n.Id == id);
            if (authorDomain != null)
            {
                _dbContext.Authors.Remove(authorDomain);
                _dbContext.SaveChanges();
            }
            return null;
        }
        public AuthorWithBooksDTO GetAuthorWithBooks(int id)
        {
            var authorData = _dbContext.Authors
                .Where(n => n.Id == id)
                .Select(author => new AuthorWithBooksDTO
                {
                    FullName = author.FullName,
                    Books = author.Book_Authors.Select(ba => ba.Book.Title).ToList()
                }).FirstOrDefault();

            return authorData;
        }
    }
}