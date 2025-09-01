
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CourseDx.Entity
{
    public class CourseEnrollment
    {
        public int Id { get; set; }
        public int CourseDetalsId { get; set; }
        public int StudentId { get; set; } 

         
        [ValidateNever]
        public Student Student { get; set; }

        [ValidateNever]
        public CourseDetals CourseDetals { get; set; }



    }
}
