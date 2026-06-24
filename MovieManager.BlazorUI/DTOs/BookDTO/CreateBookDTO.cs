using System.ComponentModel.DataAnnotations;

namespace MovieManager.BlazorUI.DTOs.BookDTO
{
    public class CreateBookDTO
    {
        [Required(ErrorMessage = "Book title is required.")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 255 characters.")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Book description is required.")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Description must be between 1 and 500 characters.")]
        public string Description { get; set; } = string.Empty;
        [Required(ErrorMessage = "Book author is required.")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Author name must be between 1 and 150 characters.")]
        public string Author { get; set; } = string.Empty;
        [Required(ErrorMessage = "Book publisher is required.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Publisher must be between 1 and 100 characters.")]
        public string Publisher { get; set; } = string.Empty;
        [Required(ErrorMessage = "Book price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public float Price { get; set; }
        public int CreatedBy { get; set; }  // Assuming this is the ID of the user creating the book
    }
}