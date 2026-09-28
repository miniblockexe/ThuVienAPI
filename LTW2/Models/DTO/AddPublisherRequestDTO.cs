using System.ComponentModel.DataAnnotations;

namespace LTW2.Models.DTO
{
    public class AddPublisherRequestDTO
    {
        [Required(ErrorMessage = "Publisher name cannot be empty")]
        public string Name { get; set; }
    }
}