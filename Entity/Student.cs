using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CourseDx.Entity
{
    public class Student
        
    {
       public int Id { get; set; }

        [DisplayName("Student Name")]
        public string Full_Name { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string Email { get; set; }  = string.Empty;
        [DisplayName("Address")]
        public string adddress { get; set; } = string.Empty;
        public ICollection<CourseEnrollment> CourseEnrollment { get; set; }

    }
}
