using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CourseDx.Entity
{
    public class CourseDetals
    {
        
        public int id {  get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime  From_Date { get; set; } = DateTime.Now;
        public DateTime To_Date { get; set; } = DateTime.Now;

        public DateTime From_Time { get; set; } = DateTime.Now;
        public DateTime To_Time { get; set; } = DateTime.Now;

        public decimal Price { get; set; } 


        public bool IsOn_line { get; set; }
        public bool IsPaid { get; set; }





        [ValidateNever]
        public Instractor Instractor { get; set; } 
        public int InstractorId { get; set; }

        [ValidateNever]
        public Course Course { get; set; }
        public int CourseId { get; set; }

        [ValidateNever]
        public virtual ICollection<CourseEnrollment> CourseEnrollment { get; set; }



    }
}
