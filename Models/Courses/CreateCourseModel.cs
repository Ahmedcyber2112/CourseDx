using System.ComponentModel.DataAnnotations;

namespace CourseDx.Models.Courses
{
    public class CreateCourseModel
    {
        //[Required]
        [Required(ErrorMessage = "Course Name is required")]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }
    }
}
