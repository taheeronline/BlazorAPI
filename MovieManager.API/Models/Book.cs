namespace MovieManager.API.Models
{
    public class Book : EntityBase
    {
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string Author { get; private set; } = string.Empty;
        public string Publisher { get; private set; } = string.Empty;
        public float Price { get; private set; }

        // Make sure these are strictly 'User'
        public User? CreatedByUser { get; private set; }
        public User? ModifiedByUser { get; private set; }


        public Book()
        {
            Title = string.Empty;
            Description = string.Empty;
            Author = string.Empty;
            Publisher = string.Empty;
            Price = 0;
        }

        public Book(string title, string description, string author, string publisher, float price)
        {
            Title = title;
            Description = description;
            Author = author;
            Publisher = publisher;
            Price = price;
        }

        public static Book Create(string title, string description, string author, string publisher, float price, int createdBy)
        {
            ValidateInputs(title, description, author, publisher, price);
            if (createdBy <= 0) throw new ArgumentException("Invalid User ID.", nameof(createdBy));

            var book = new Book(title, description, author, publisher, price)
            {
                CreatedDate= DateTime.UtcNow,
                IsDeleted=false,
                CreatedBy = createdBy
            };
            return book;
        }

        public void Update(string title, string description, string author, string publisher, float price)
        {
            ValidateInputs(title, description, author, publisher, price);

            if (Id <= 0) throw new ArgumentException("Invalid Book ID.", nameof(Id));
            
            this.Title = title;
            this.Description = description;
            this.Author = author;
            this.Publisher = publisher;
            this.Price = price;
            
            UpdateLastModified();
        }

        private static void ValidateInputs(string title, string description, string author, string publisher, float price)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be null or empty.", nameof(description));
            if (string.IsNullOrWhiteSpace(author))
                throw new ArgumentException("Author cannot be null or empty.", nameof(author));
            if (string.IsNullOrWhiteSpace(publisher))
                throw new ArgumentException("Publisher cannot be null or empty.", nameof(publisher));
            if (price < 0)
                throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");
        }
    }
}
