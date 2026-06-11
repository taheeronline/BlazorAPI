namespace MovieManager.API.Models
{
    public abstract class EntityBase
    {
        public int Id { get; protected set; }

        public DateTime CreatedDate { get; protected set; }
        public DateTime? ModifiedDate { get; protected set; }
        public bool IsDeleted { get; protected set; }

        public void MarkAsDeleted()
        {
            IsDeleted = true;
            UpdateLastModified();
        }

        protected void UpdateLastModified()
        {
            ModifiedDate = DateTime.UtcNow;
        }
    }
}