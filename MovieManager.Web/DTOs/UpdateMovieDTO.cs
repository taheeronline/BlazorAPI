using System;
using System.ComponentModel.DataAnnotations;

namespace MovieManager.Web.DTOs
{
    /// <summary>
    /// Data Transfer Object for updating an existing movie.
    /// Contains the properties that can be modified in a movie.
    /// </summary>
    public class UpdateMovieDTO
    {
        /// <summary>
        /// Updated title of the movie (required).
        /// </summary>
        [Required(ErrorMessage = "Movie title is required.")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 255 characters.")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Updated director of the movie (required).
        /// </summary>
        [Required(ErrorMessage = "Movie director is required.")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Director name must be between 1 and 150 characters.")]
        public string Director { get; set; } = string.Empty;

        /// <summary>
        /// Updated genre/category of the movie (required).
        /// </summary>
        [Required(ErrorMessage = "Movie genre is required.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Genre must be between 1 and 100 characters.")]
        public string Genre { get; set; } = string.Empty;

        /// <summary>
        /// Updated release date of the movie (required).
        /// </summary>
        [Required(ErrorMessage = "Release date is required.")]
        public DateTimeOffset ReleaseDate { get; set; }

        /// <summary>
        /// Updated rating of the movie on a scale of 0-10 (required).
        /// </summary>
        [Required(ErrorMessage = "Movie rating is required.")]
        [Range(0, 10, ErrorMessage = "Rating must be between 0 and 10.")]
        public double Rating { get; set; }

        /// <summary>
        /// Unique identifier of the user modifying the record (required).
        /// </summary>
        [Required(ErrorMessage = "ModifiedBy identifier is required.")]
        public Guid ModifiedBy { get; set; }
    }
}