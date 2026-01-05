using System.Collections.Generic;
using CourseDx.Entity;

namespace CourseDx.Models
{
    public class CourseDiscoveryViewModel
    {
        public List<Course> Courses { get; set; } = new List<Course>();
        public List<CourseDetals> CourseDetails { get; set; } = new List<CourseDetals>();
        public List<Instractor> Instructors { get; set; } = new List<Instractor>();
        public List<CourseDx.Entity.CourseEnrollment> Enrollments { get; set; } = new List<CourseDx.Entity.CourseEnrollment>();
    }
}
