using LTW2.Models.Domain;
using LTW2.Models.DTO;

namespace LTW2.Repositories
{
    public interface IPublisherRepository
    {
        List<PublisherDTO> GetAllPublishers(string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000);
        PublisherNoIdDTO GetPublisherById(int id);
        AddPublisherRequestDTO AddPublisher(AddPublisherRequestDTO addPublisherRequestDTO);
        PublisherNoIdDTO UpdatePublisherById(int id, PublisherNoIdDTO publisherNoIdDTO);
        Publisher? DeletePublisherById(int id);
        PublisherWithBooksAndAuthorsDTO GetPublisherData(int id);
    }
}