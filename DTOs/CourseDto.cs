using System.ComponentModel.DataAnnotations;

namespace CourseDx.DTOs
{
    /// <summary>
    /// DTO for Course data transfer
    /// </summary>
    public class CourseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SessionCount { get; set; }
        public int InstructorCount { get; set; }
    }

    /// <summary>
    /// DTO for creating a new course
    /// </summary>
    public class CreateCourseDto
    {
        [Required(ErrorMessage = "Course name is required")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Course name must be between 2 and 150 characters")]
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for updating a course
    /// </summary>
    public class UpdateCourseDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Course name is required")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Course name must be between 2 and 150 characters")]
        public string Name { get; set; } = string.Empty;
    }
}
