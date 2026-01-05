using FluentValidation;
using CourseDx.DTOs;

namespace CourseDx.Validators
{
    /// <summary>
    /// Validator for CreateCourseDto
    /// </summary>
    public class CreateCourseValidator : AbstractValidator<CreateCourseDto>
    {
        public CreateCourseValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Course name is required")
                .MinimumLength(2).WithMessage("Course name must be at least 2 characters")
                .MaximumLength(150).WithMessage("Course name cannot exceed 150 characters");
        }
    }

    /// <summary>
    /// Validator for UpdateCourseDto
    /// </summary>
    public class UpdateCourseValidator : AbstractValidator<UpdateCourseDto>
    {
        public UpdateCourseValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid course ID");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Course name is required")
                .MinimumLength(2).WithMessage("Course name must be at least 2 characters")
                .MaximumLength(150).WithMessage("Course name cannot exceed 150 characters");
        }
    }
}
