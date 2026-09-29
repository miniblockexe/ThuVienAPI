using System.ComponentModel.DataAnnotations;
using LTW2.Models.Domain;

namespace LTW2.Models.DTO
{
    public class AddBookRequestDTO
    {
        [Required(ErrorMessage = "Title cannot be empty")]
        [MinLength(10)]
        [RegularExpression(@"^[a-zA-Z0-9\sàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđĐ.,;@!\-_]*$", ErrorMessage = "Title cannot contain special characters")]
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        [Range(0, 5, ErrorMessage = "From 0 to 5")]
        public int? Rate { get; set; }
        public string? Genre { get; set; }
        public string? CoverUrl { get; set; }
        public DateTime DateAdded { get; set; }

        //navigation Properties -
        public int PublisherID { get; set; }
        public List<int> AuthorIds { get; set; }
    }
}