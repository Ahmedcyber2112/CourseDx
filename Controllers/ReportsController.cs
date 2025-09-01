using ClosedXML.Excel;
using CourseDx.Data;
using CourseDx.Entity;
using CourseDx.Models.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;

namespace CourseDx.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly CourseDxContext _context;

        public ReportsController(CourseDxContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Search()
        {
            var model = new SearchViewModel();
            LoadDropdowns(model);

            return View(model);
        }

        [HttpPost]
        public IActionResult Search(SearchViewModel model)
        {
            model.Results = GetData(model);
            LoadDropdowns(model);

            return View(model);
        }

        [HttpPost]
        public IActionResult ExportToExcel(SearchViewModel model)
        {
            var CourseDetails = GetData(model);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("الدورات");
                worksheet.RightToLeft = true;

                // Headers
                worksheet.Cell(1, 1).Value = "مسلسل";
                worksheet.Cell(1, 2).Value = "الكورس";
                worksheet.Cell(1, 3).Value = "الوصف";
                worksheet.Cell(1, 4).Value = "تاريخ البداية";
                worksheet.Cell(1, 5).Value = "تاريخ الانتهاء";
                worksheet.Cell(1, 6).Value = "المحاضر";
                worksheet.Cell(1, 7).Value = "الوقت";
                worksheet.Cell(1, 8).Value = "السعر";
                worksheet.Cell(1, 9).Value = "نوع الدورة";

                var headerRange = worksheet.Range(1, 1, 1, 9);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Font.FontName = "Arial";

                int row = 2;
                int serial = 1;
                foreach (var c_detail in CourseDetails)
                {
                    worksheet.Cell(row, 1).Value = serial;
                    worksheet.Cell(row, 2).Value = c_detail.Course.Name;
                    worksheet.Cell(row, 3).Value = c_detail.Title;
                    worksheet.Cell(row, 4).Value = c_detail.From_Date.ToString("yyyy/MM/dd");
                    worksheet.Cell(row, 5).Value = c_detail.To_Date.ToString("yyyy/MM/dd");
                    worksheet.Cell(row, 6).Value = c_detail.Instractor.Name;
                    worksheet.Cell(row, 7).Value = $"{c_detail.From_Time} - {c_detail.To_Time}";
                    worksheet.Cell(row, 8).Value = c_detail.Price;
                    worksheet.Cell(row, 9).Value = c_detail.IsOn_line ? "أونلاين" : "حضوري";
                    row++;
                    serial++;
                }

                worksheet.Columns().AdjustToContents();

                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                return File(
                    stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"الكورسات-{DateTime.Now:yyyyMMddHHmmss}.xlsx"
                );
            }
        }

        private List<CourseDetals> GetData(SearchViewModel model)
        {
            var results = _context.CourseDetals
                 .Include(cd => cd.Course)
                 .Include(cd => cd.Instractor)
                 .Include(s => s.CourseEnrollment)
                 .AsQueryable();

            // Online status filter
            if (model.OnlineStatus.HasValue)
                results = results.Where(cd => cd.IsOn_line == (model.OnlineStatus.Value == OnlineStatus.Online));

            // Students filter (multi-select)
            if (model.StudentIds != null && model.StudentIds.Any())
                results = results.Where(cd => cd.CourseEnrollment.Any(e => model.StudentIds.Contains(e.StudentId)));

            // Instructors filter (multi-select)
            if (model.InstructorIds != null && model.InstructorIds.Any())
                results = results.Where(cd => model.InstructorIds.Contains(cd.InstractorId));

            // Courses filter (multi-select)
            if (model.CourseIds != null && model.CourseIds.Any())
                results = results.Where(cd => model.CourseIds.Contains(cd.CourseId));

            // Date range filter
            if (model.StartDate.HasValue || model.EndDate.HasValue)
            {
                if (model.StartDate.HasValue && !model.EndDate.HasValue)
                {
                    results = results.Where(cd => cd.To_Date >= model.StartDate.Value);
                }
                else if (!model.StartDate.HasValue && model.EndDate.HasValue)
                {
                    results = results.Where(cd => cd.From_Date <= model.EndDate.Value);
                }
                else if (model.StartDate.HasValue && model.EndDate.HasValue)
                {
                    results = results.Where(cd =>
                        cd.From_Date <= model.EndDate.Value &&
                        cd.To_Date >= model.StartDate.Value);
                }
            }

            // Time range filter
            if (!string.IsNullOrEmpty(model.StartTime) || !string.IsNullOrEmpty(model.EndTime))
            {
                if (!string.IsNullOrEmpty(model.StartTime) && string.IsNullOrEmpty(model.EndTime))
                {
                    if (TimeSpan.TryParse(model.StartTime, out TimeSpan startTime))
                        results = results.Where(cd => cd.From_Time.TimeOfDay >= startTime);
                }
                else if (string.IsNullOrEmpty(model.StartTime) && !string.IsNullOrEmpty(model.EndTime))
                {
                    if (TimeSpan.TryParse(model.EndTime, out TimeSpan endTime))
                        results = results.Where(cd => cd.To_Time.TimeOfDay <= endTime);
                }
                else if (!string.IsNullOrEmpty(model.StartTime) && !string.IsNullOrEmpty(model.EndTime))
                {
                    if (TimeSpan.TryParse(model.StartTime, out TimeSpan startTime) &&
                        TimeSpan.TryParse(model.EndTime, out TimeSpan endTime))
                    {
                        if (startTime <= endTime)
                        {
                            results = results.Where(cd =>
                                cd.From_Time.TimeOfDay <= endTime &&
                                cd.To_Time.TimeOfDay >= startTime);
                        }
                    }
                }
            }

            // Price range filter
            if (model.MinPrice.HasValue)
                results = results.Where(cd => cd.Price >= model.MinPrice.Value);

            if (model.MaxPrice.HasValue)
                results = results.Where(cd => cd.Price <= model.MaxPrice.Value);

            return results.ToList();
        }

        private void LoadDropdowns(SearchViewModel model)
        {
            model.Students = _context.Students
               .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Full_Name })
               .ToList();

            model.Instructors = _context.Instractor
              .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
              .ToList();

            model.Courses = _context.Courses
              .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
              .ToList();
        }

        [HttpPost]
        public IActionResult ExportToPDF(SearchViewModel model)
        {
            var Data = GetData(model);

            return new ViewAsPdf("SearchToPdf", Data)
            {
                FileName = $"Report-{DateTime.Now:yyyyMMddHHmmss}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageMargins = { Left = 10, Right = 10, Top = 10, Bottom = 10 },
                CustomSwitches = "--footer-center \"[page] of [toPage]\" --footer-font-size 9"
            };
        }
    }
}
