using System.ComponentModel.DataAnnotations;

namespace LTW2.Models.DTO
{
    public class AddAuthorRequestDTO
    {
        [Required(ErrorMessage = "Author name cannot be empty")]
        [MinLength(3, ErrorMessage = "Author name must be at least 3 characters")]
        public string FullName { get; set; }
    }
}