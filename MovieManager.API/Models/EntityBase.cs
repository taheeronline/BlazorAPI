namespace MovieManager.API.Models
{
    public class EntityBase
    {
        public Guid Id { get; set; }= Guid.NewGuid();
        public DateTimeOffset Created { get; set; }= DateTimeOffset.Now;
        public DateTimeOffset LastModified { get; set; } = DateTimeOffset.UtcNow;

        public void UpdateLastModified()
        {
            LastModified = DateTimeOffset.UtcNow;
        }
    }
}
