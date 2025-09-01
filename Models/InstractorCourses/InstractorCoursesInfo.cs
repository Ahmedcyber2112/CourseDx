using System.ComponentModel.DataAnnotations;

namespace CourseDx.Models.InstractorCourses
{
    public class InstractorCoursesInfo
    {
        public int Id { get; set; } 
        public string CourseName { get; set; }
        public string InstructorName { get; set; }

    }
}
