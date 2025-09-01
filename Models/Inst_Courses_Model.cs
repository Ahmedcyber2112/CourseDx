using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CourseDx.Models
{
    public class Inst_Courses_Model
    { 

        [Required(ErrorMessage = "Please select a course")]
        [Display(Name = "Course")]
        public int CourseId { get; set; }  // Changed to CourseId (consistent naming)

      

        [Required(ErrorMessage = "Please select an instructor")]
        [Display(Name = "Instructor")]
        public int InstructorId { get; set; }  // Changed to InstructorId

 
    }
}
