using System.ComponentModel.DataAnnotations;

namespace MovieManager.API.DTOs.MovieDTOs
{
    public class CreateMovieDTO
    {
        [Required(ErrorMessage = "Movie title is required.")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 255 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Movie director is required.")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Director name must be between 1 and 150 characters.")]
        public string Director { get; set; } = string.Empty;

        [Required(ErrorMessage = "Movie genre is required.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Genre must be between 1 and 100 characters.")]
        public string Genre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Release date is required.")]
        public DateTimeOffset ReleaseDate { get; set; }

        [Required(ErrorMessage = "Movie rating is required.")]
        [Range(0, 10, ErrorMessage = "Rating must be between 0 and 10.")]
        public double Rating { get; set; }

        public int CreatedBy { get; set; }  // Assuming this is the ID of the user creating the movie
    }
}