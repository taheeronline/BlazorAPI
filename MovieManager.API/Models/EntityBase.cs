namespace MovieManager.API.Models
{
    public abstract class EntityBase
    {
        public int Id { get; protected set; }

        public DateTime CreatedDate { get; protected set; }
        public DateTime? ModifiedDate { get; protected set; }
        public bool IsDeleted { get; protected set; }

        public int? CreatedBy { get; protected set; }
        public int? ModifiedBy { get; protected set; }

        public void MarkAsDeleted(int modifiedById)
        {
            IsDeleted = true;
            ModifiedBy = modifiedById;
            UpdateLastModified();
        }

        protected void UpdateLastModified()
        {
            ModifiedDate = DateTime.UtcNow;
        }
    }
}