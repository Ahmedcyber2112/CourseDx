using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CourseDx.Entity
{
    public class InstractorCourses
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int InstractorId { get; set; }

        [ValidateNever]
        public virtual Course Course { get; set; }

        [ValidateNever]
        public virtual Instractor Instractor { get; set; }


    }
}
