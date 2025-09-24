using StudentDocManagement.Entity.Dto;
using FluentValidation;

namespace StudentDocManagement.Entity.Validator
{
    public class StudentProfileDtoValidator: AbstractValidator<StudentProfileDto>
    {
        public StudentProfileDtoValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.DOB)
     .NotEmpty().WithMessage("Date of Birth is required")
     .Must(d => d.Date <= DateTime.Today)
         .WithMessage("Date of Birth must be today or in the past")
     .Must(d => d <= DateTime.Today.AddYears(-15))
         .WithMessage("Student must be at least 15 years old");

            RuleFor(x => x.GenderId)
                .NotEmpty().WithMessage("Gender is required");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^[6-9]\d{9}$").WithMessage("Invalid phone number");

            RuleFor(x => x.AlternatePhoneNumber)
                .Matches(@"^[6-9]\d{9}$")
                    .When(x => !string.IsNullOrEmpty(x.AlternatePhoneNumber))
                    .WithMessage("Invalid alternate phone number")
                .NotEqual(x => x.PhoneNumber)
                    .When(x => !string.IsNullOrEmpty(x.AlternatePhoneNumber))
                    .WithMessage("Alternate phone number must be different from primary phone number");


            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required")
                .MaximumLength(300).WithMessage("Address cannot exceed 300 characters");

            RuleFor(x => x.PermanentAddress)
                .NotEmpty().WithMessage("Permanent Address is required")
                .MaximumLength(300).WithMessage("Permanent Address cannot exceed 300 characters");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required")
                .MaximumLength(100).WithMessage("City cannot exceed 100 characters");

            RuleFor(x => x.District)
                .NotEmpty().WithMessage("District is required")
                .MaximumLength(100).WithMessage("District cannot exceed 100 characters");

            RuleFor(x => x.State)
                .NotEmpty().WithMessage("State is required")
                .MaximumLength(100).WithMessage("State cannot exceed 100 characters");

            //RuleFor(x => x.Pincode)
            //    .NotEmpty().WithMessage("Pincode is required")
            //    .Length(6).WithMessage("Pincode must be exactly 6 digits")
            //    .Matches(@"^\d{6}$").WithMessage("Pincode must contain digits only")
            //    .Matches(@"^[1-9]\d{5}$").WithMessage("Pincode cannot start with 0");


            RuleFor(x => x.IdProofTypeId)
                .GreaterThan(0).WithMessage("IdProofTypeId is required");

            RuleFor(x => x.IdProofNumber)
                .NotEmpty().WithMessage("Id Proof Number is required")
                .MaximumLength(50).WithMessage("Id Proof Number cannot exceed 50 characters");

            RuleFor(x => x.CourseId)
                .GreaterThan(0).WithMessage("CourseId is required");
        }
    }
}
