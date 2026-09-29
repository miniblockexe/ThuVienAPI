using System.ComponentModel.DataAnnotations;

namespace LTW2.Models.DTO
{
    public class AddBookAuthorRequestDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "BookId phải là số nguyên dương")]
        public int BookId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "AuthorId phải là số nguyên dương")]
        public int AuthorId { get; set; }
    }
}