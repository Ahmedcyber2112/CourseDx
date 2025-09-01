using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CourseDx.Entity
{
    public class Instractor
    {
        public int Id { get; set; }
        [DisplayName("Instructor Name")]
        public string Name { get; set; }  

        public string Title { get; set; } 

        public string Description { get; set; }

        [Display(Name = "Gender")]
        public int Gender { get; set; } 

        // علاقة One-to-Many مع CourseDetals
        public ICollection<CourseDetals> CourseDetals { get; set; } = new List<CourseDetals>();

        // علاقة Many-to-Many مع الكورسات من خلال جدول InstractorCourses
        public ICollection<InstractorCourses> InstractorCourses { get; set; } = new List<InstractorCourses>();

       
    }
}
