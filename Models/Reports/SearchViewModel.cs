using CourseDx.Entity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CourseDx.Models.Reports
{
    public class SearchViewModel
    {
        [Required(ErrorMessage = "Please select an option")]
        public OnlineStatus? OnlineStatus { get; set; }

        // ✅ بدل int? خليها List<int> عشان multi-select
        public List<int>? StudentIds { get; set; }
        public List<SelectListItem>? Students { get; set; }

        public List<int>? InstructorIds { get; set; }
        public List<SelectListItem>? Instructors { get; set; }

        public List<int>? CourseIds { get; set; }
        public List<SelectListItem>? Courses { get; set; }

        public List<CourseDetals>? Results { get; set; }

        // Date range
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Time range
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }

        // Price range
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }

    public enum OnlineStatus
    {
        Offline,
        Online
    }
}
