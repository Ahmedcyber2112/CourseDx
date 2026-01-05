using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CourseDx.Entity
{
    public class Instractor
    {
        public int Id { get; set; }
        [DisplayName("Instructor Name")]
        public string Name { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Display(Name = "Gender")]
        public int Gender { get; set; } 

        // One-to-Many relationship with CourseDetals
        public ICollection<CourseDetals> CourseDetals { get; set; } = new List<CourseDetals>();

        // Many-to-Many relationship with Courses through InstractorCourses table
        public ICollection<InstractorCourses> InstractorCourses { get; set; } = new List<InstractorCourses>();

       
    }
}
