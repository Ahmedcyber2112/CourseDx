using System.Collections.Generic;
using CourseDx.Entity;

namespace CourseDx.Models
{
    public class CourseDiscoveryViewModel
    {
        public List<Course> Courses { get; set; }
        public List<CourseDetals> CourseDetails { get; set; }
        public List<Instractor> Instructors { get; set; }
        public List<CourseDx.Entity.CourseEnrollment> Enrollments { get; set; }
    }
}
