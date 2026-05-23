namespace MovieManager.Web.DTOs
{
    /// <summary>
    /// Data Transfer Object for Movie entity.
    /// Used for returning complete movie information from API endpoints.
    /// </summary>
    public class MovieDTO
    {
        /// <summary>
        /// Unique identifier for the movie.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Title of the movie.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Director of the movie.
        /// </summary>
        public string Director { get; set; } = string.Empty;

        /// <summary>
        /// Genre/Category of the movie.
        /// </summary>
        public string Genre { get; set; } = string.Empty;

        /// <summary>
        /// Release date of the movie.
        /// </summary>
        public DateTimeOffset ReleaseDate { get; set; }

        /// <summary>
        /// Rating of the movie (0-10 scale).
        /// </summary>
        public double Rating { get; set; }

        /// <summary>
        /// Timestamp when the record was created.
        /// </summary>
        public DateTimeOffset Created { get; set; }

        /// <summary>
        /// Timestamp when the record was last modified.
        /// </summary>
        public DateTimeOffset LastModified { get; set; }
    }
}
