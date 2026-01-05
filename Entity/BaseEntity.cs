namespace CourseDx.Entity
{
    /// <summary>
    /// Base entity with audit trail and soft delete support
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Date and time when the entity was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// User who created the entity
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Date and time when the entity was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// User who last updated the entity
        /// </summary>
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Indicates if the entity is deleted (soft delete)
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Date and time when the entity was deleted
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// User who deleted the entity
        /// </summary>
        public string? DeletedBy { get; set; }
    }
}
