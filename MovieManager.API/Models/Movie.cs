using Microsoft.Extensions.Options;

namespace MovieManager.API.Models
{
    public class Movie : EntityBase
    {
        public string Title { get; set; }

        public string Director { get; set; }
        public string Genre { get; set; }
        public DateTimeOffset ReleaseDate { get; set; }

        public double Rating { get; set; } = 0;

        private Movie() 
        {
            Title = string.Empty;
            Director = string.Empty;
            Genre = string.Empty;
        }

        // use different parameter names (camelCase) to avoid shadowing properties
        private Movie(string title, string director, string genre, DateTimeOffset releaseDate, double rating)
        {
            Title = title;
            Director = director;
            Genre = genre;
            ReleaseDate = releaseDate;
            Rating = rating;
        }

        public static Movie Create(string title, string director, string genre, DateTimeOffset releaseDate, double rating)
        {
            ValidateInputs(title, director, genre, releaseDate, rating);

            return new Movie(title, director, genre, releaseDate, rating);
        }

        // Make Update an instance method so UpdateLastModified() can be called
        public void Update(string title, string director, string genre, DateTimeOffset releaseDate, double rating)
        {
            ValidateInputs(title, director, genre, releaseDate, rating);

            // assign to instance properties explicitly
            this.Title = title;
            this.Director = director;
            this.Genre = genre;
            this.ReleaseDate = releaseDate;
            this.Rating = rating;

            // now valid: instance method call
            UpdateLastModified();
        }

        // add return type (void) and keep static since it doesn't need instance state
        private static void ValidateInputs(string title, string director, string genre, DateTimeOffset releaseDate, double rating)
        { 
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentException("Title cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(director))
            {
                throw new ArgumentException("Director cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(genre))
            {
                throw new ArgumentException("Genre cannot be null or empty.");
            }
            if (rating < 0 || rating > 10)
            {
                throw new ArgumentException("Rating must be between 0 and 10.");
            }
        }
    }
}