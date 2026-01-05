using System.ComponentModel.DataAnnotations;

namespace CourseDx.Models.InstractorCourses
{
    public class InstractorCoursesInfo
    {
        public int Id { get; set; } 
        public string CourseName { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;

    }
}
