using System.Collections.Generic;

namespace CourseDx.Models.Admin
{
    // A simple class to hold data points for the charts
    // فئة بسيطة لتخزين نقاط البيانات للمخططات
    public class ChartDataPoint
    {
        public string Label { get; set; } // Example: "Jan", "Feb" or "C# Course"
        public decimal Value { get; set; } // Example: 150 (students) or 12000 (revenue)
    }

    public class HomeStatisticsModel
    {
        // You can keep the old counts if you need them elsewhere
        // يمكنك الإبقاء على الإحصائيات القديمة إذا كنت تحتاجها في مكان آخر
        public int StudentsCount { get; set; }
        public int EnrollmentsCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public int InstractorsCount { get; set; } // Typo from original code, consider renaming to InstructorsCount

        // New properties to hold data for the ApexCharts
        // خصائص جديدة لتمرير البيانات لمخططات ApexCharts
        public List<ChartDataPoint> StudentGrowthData { get; set; }
        public List<ChartDataPoint> CourseEnrollmentData { get; set; }
        public List<ChartDataPoint> RevenueGrowthData { get; set; }
        public List<ChartDataPoint> InstructorGrowthData { get; set; }
    }
}