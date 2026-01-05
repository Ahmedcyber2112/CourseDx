using FluentValidation;
using CourseDx.DTOs;

namespace CourseDx.Validators
{
    /// <summary>
    /// Validator for CreateInstructorDto
    /// </summary>
    public class CreateInstructorValidator : AbstractValidator<CreateInstructorDto>
    {
        public CreateInstructorValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Instructor name is required")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z\s\u0600-\u06FF]+$").WithMessage("Name can only contain letters and spaces");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.Gender)
                .InclusiveBetween(0, 2).WithMessage("Gender must be 0 (Male), 1 (Female), or 2 (Other)");
        }
    }

    /// <summary>
    /// Validator for UpdateInstructorDto
    /// </summary>
    public class UpdateInstructorValidator : AbstractValidator<UpdateInstructorDto>
    {
        public UpdateInstructorValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid instructor ID");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Instructor name is required")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z\s\u0600-\u06FF]+$").WithMessage("Name can only contain letters and spaces");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.Gender)
                .InclusiveBetween(0, 2).WithMessage("Gender must be 0 (Male), 1 (Female), or 2 (Other)");
        }
    }
}
