using Microsoft.EntityFrameworkCore;
using LTW2.Data;
using LTW2.Models.Domain;
using LTW2.Models.DTO;

namespace LTW2.Repositories
{
    public class SQLPublisherRepository : IPublisherRepository
    {
        private readonly AppDbContext _dbContext;

        public SQLPublisherRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<PublisherDTO> GetAllPublishers(string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
        {
            //Get Data From Database -Domain Model
            var allPublishers = _dbContext.Publishers.Select(p => new PublisherDTO
            {
                Id = p.Id,
                Name = p.Name
            }).AsQueryable();
            // filtering
            if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
            {
                if (filterOn.Equals("name", StringComparison.OrdinalIgnoreCase))
                {
                    allPublishers = allPublishers.Where(x => x.Name.Contains(filterQuery));
                }
            }
            // sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                if (sortBy.Equals("name", StringComparison.OrdinalIgnoreCase))
                {
                    allPublishers = isAscending
                        ? allPublishers.OrderBy(x => x.Name)
                        : allPublishers.OrderByDescending(x => x.Name);
                }
            }
            // pagination
            var skipResults = (pageNumber - 1) * pageSize;
            return allPublishers.Skip(skipResults).Take(pageSize).ToList();
        }

        public PublisherNoIdDTO GetPublisherById(int id)
        {
            // get book Domain model from Db
            var publisherWithIdDomain = _dbContext.Publishers.FirstOrDefault(x => x.Id == id);

            if (publisherWithIdDomain != null)
            {
                //Map Domain Model to DTOs
                var publisherNoIdDTO = new PublisherNoIdDTO
                {
                    Name = publisherWithIdDomain.Name,
                };

                return publisherNoIdDTO;
            }

            return null;
        }

        public AddPublisherRequestDTO AddPublisher(AddPublisherRequestDTO addPublisherRequestDTO)
        {
            var publisherDomainModel = new Publisher
            {
                Name = addPublisherRequestDTO.Name,
            };

            //Use Domain Model to create Book
            _dbContext.Publishers.Add(publisherDomainModel);
            _dbContext.SaveChanges();
            return addPublisherRequestDTO;
        }

        public PublisherNoIdDTO UpdatePublisherById(int id, PublisherNoIdDTO publisherNoIdDTO)
        {
            var publisherDomain = _dbContext.Publishers.FirstOrDefault(n => n.Id == id);
            if (publisherDomain != null)
            {
                publisherDomain.Name = publisherNoIdDTO.Name;

                _dbContext.SaveChanges();
            }
            return null;
        }

        public Publisher? DeletePublisherById(int id)
        {
            var publisherDomain = _dbContext.Publishers.FirstOrDefault(n => n.Id == id);
            if (publisherDomain != null)
            {
                _dbContext.Publishers.Remove(publisherDomain);
                _dbContext.SaveChanges();
            }
            return null;
        }
        public PublisherWithBooksAndAuthorsDTO GetPublisherData(int id)
        {
            var publisherData = _dbContext.Publishers
                .Where(n => n.Id == id)
                .Select(publisher => new PublisherWithBooksAndAuthorsDTO
                {
                    Name = publisher.Name,
                    BookAuthors = publisher.Books.Select(book => new BookAuthorDTO
                    {
                        BookName = book.Title,
                        BookAuthors = book.Book_Authors.Select(a => a.Author.FullName).ToList()
                    }).ToList()
                }).FirstOrDefault();

            return publisherData;
        }
    }
}