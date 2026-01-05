using System.ComponentModel.DataAnnotations;

namespace CourseDx.DTOs
{
    /// <summary>
    /// DTO for Instructor data transfer
    /// </summary>
    public class InstructorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int CourseCount { get; set; }
    }

    /// <summary>
    /// DTO for creating a new instructor
    /// </summary>
    public class CreateInstructorDto
    {
        [Required(ErrorMessage = "Instructor name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Range(0, 2, ErrorMessage = "Gender must be 0 (Male), 1 (Female), or 2 (Other)")]
        public int Gender { get; set; }
    }

    /// <summary>
    /// DTO for updating an instructor
    /// </summary>
    public class UpdateInstructorDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Instructor name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Range(0, 2, ErrorMessage = "Gender must be 0 (Male), 1 (Female), or 2 (Other)")]
        public int Gender { get; set; }
    }
}
