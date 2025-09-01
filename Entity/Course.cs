using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;

namespace CourseDx.Entity
{
    public class Course
    {
        public int Id { get; set; }
        [DisplayName("Course Name")]
        public string Name { get; set; }
        
        [ValidateNever]
        public virtual ICollection<CourseDetals> CourseDetals { get; set; }
        [ValidateNever]
        public virtual ICollection<InstractorCourses> InstractorCourses { get; set; }


    }
}
