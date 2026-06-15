
namespace MovieManager.API.Models
{
    public class Movie : EntityBase
    {
        public string Title { get; private set; }
        public string Director { get; private set; }
        public string Genre { get; private set; }
        public DateTimeOffset ReleaseDate { get; private set; }
        public double Rating { get; private set; } = 0;

        // Make sure these are strictly 'User'
        public User? CreatedByUser { get; private set; }
        public User? ModifiedByUser { get; private set; }

        private Movie()
        {
            Title = string.Empty;
            Director = string.Empty;
            Genre = string.Empty;
        }

        private Movie(string title, string director, string genre, DateTimeOffset releaseDate, double rating)
        {
            Title = title;
            Director = director;
            Genre = genre;
            ReleaseDate = releaseDate;
            Rating = rating;
        }

        public static Movie Create(string title, string director, string genre, DateTimeOffset releaseDate, double rating, int userId)
        {
            ValidateInputs(title, director, genre, releaseDate, rating);
            if (userId <= 0) throw new ArgumentException("Invalid User ID.", nameof(userId));

            var movie = new Movie(title, director, genre, releaseDate, rating)
            {
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false,
                CreatedBy = userId
            };

            return movie;
        }

        public void Update(string title, string director, string genre, DateTimeOffset releaseDate, double rating, int userId)
        {
            ValidateInputs(title, director, genre, releaseDate, rating);
            if (userId <= 0) throw new ArgumentException("Invalid User ID.", nameof(userId));

            this.Title = title;
            this.Director = director;
            this.Genre = genre;
            this.ReleaseDate = releaseDate;
            this.Rating = rating;

            this.ModifiedBy = userId;

            UpdateLastModified();
        }

        private static void ValidateInputs(string title, string director, string genre, DateTimeOffset releaseDate, double rating)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title cannot be empty.", nameof(title));
            if (string.IsNullOrWhiteSpace(director)) throw new ArgumentException("Director cannot be empty.", nameof(director));
            if (string.IsNullOrWhiteSpace(genre)) throw new ArgumentException("Genre cannot be empty.", nameof(genre));
            if (releaseDate == DateTimeOffset.MinValue) throw new ArgumentException("Invalid release date.", nameof(releaseDate));
            if (rating < 0 || rating > 10) throw new ArgumentException("Rating must be between 0 and 10.", nameof(rating));
        }
    }
}