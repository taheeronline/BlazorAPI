namespace MovieManager.API.Models
{
    public class Movie : EntityBase
    {
        public string Title { get; private set; }
        public string Director { get; private set; }
        public string Genre { get; private set; }
        public DateTimeOffset ReleaseDate { get; private set; }
        public double Rating { get; private set; } = 0;

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

        public static Movie Create(string title, string director, string genre, DateTimeOffset releaseDate, double rating)
        {
            ValidateInputs(title, director, genre, releaseDate, rating);

            var movie = new Movie(title, director, genre, releaseDate, rating)
            {
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };

            return movie;
        }

        public void Update(string title, string director, string genre, DateTimeOffset releaseDate, double rating)
        {
            ValidateInputs(title, director, genre, releaseDate, rating);

            this.Title = title;
            this.Director = director;
            this.Genre = genre;
            this.ReleaseDate = releaseDate;
            this.Rating = rating;

            UpdateLastModified();
        }

        private static void ValidateInputs(string title, string director, string genre, DateTimeOffset releaseDate, double rating)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(director)) throw new ArgumentException("Director cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(genre)) throw new ArgumentException("Genre cannot be null or empty.");
            if (rating < 0 || rating > 10) throw new ArgumentException("Rating must be between 0 and 10.");
        }
    }
}