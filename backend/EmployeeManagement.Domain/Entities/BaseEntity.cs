using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Domain.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime? UpdatedAt { get; private set; }

        public DateTime? DeletedAt { get; private set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsDeleted()
        {
            DeletedAt = DateTime.UtcNow; 
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
