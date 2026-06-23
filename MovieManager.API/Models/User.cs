namespace MovieManager.API.Models
{
    public class User: EntityBase
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string HashPassword { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        private User()
        {
            Name = string.Empty;
            UserName = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            HashPassword = string.Empty;
            Role = string.Empty;
        }

        private User(string name, string userName, string email, string password, string hashPassword, string role, int createdBy)
        {
            Name = name;
            UserName = userName;
            Email = email;
            Password = password;
            HashPassword = hashPassword;
            Role = role;
            CreatedBy = createdBy;
        }

        public static User Create(string name, string userName, string email, string password, string hashPassword, string role, int createdBy)
        {
            ValidateInputs(name, userName, email, password, hashPassword, role);

            var user = new User(name, userName, email, password, hashPassword, role, createdBy)
            {
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };

            return user;
        }

        public void Update(string name, string userName, string email, string role, int modifiedBy)
        {
            ValidateInputs(name, userName, email, role);
            if (Id <= 0) throw new ArgumentException("Invalid User ID.", nameof(Id));

            this.Name = name;
            this.UserName = userName;
            this.Email = email;
            this.Role = role;
            this.ModifiedBy = modifiedBy;
            UpdateLastModified();    
        }

        private static void ValidateInputs(string name, string userName, string email, string role)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty.", nameof(name));
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("Username cannot be empty.", nameof(userName));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email cannot be empty.", nameof(email));
            if (string.IsNullOrWhiteSpace(role)) throw new ArgumentException("Role cannot be empty.", nameof(role));
        }

        private static void ValidateInputs(string name, string userName, string email, string password, string hashPassword, string role)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty.", nameof(name));
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("Username cannot be empty.", nameof(userName));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email cannot be empty.", nameof(email));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password cannot be empty.", nameof(password));
            if (string.IsNullOrWhiteSpace(hashPassword)) throw new ArgumentException("Hashed password cannot be empty.", nameof(hashPassword));
            if (string.IsNullOrWhiteSpace(role)) throw new ArgumentException("Role cannot be empty.", nameof(role));
        }
    }
}           