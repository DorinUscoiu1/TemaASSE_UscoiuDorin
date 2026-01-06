using Domain.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Validators
{
    public class EditionValidator : AbstractValidator<Edition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditionValidator"/> class.
        /// </summary>
        public EditionValidator()
        {
            this.RuleFor(e => e.Publisher)
                .NotEmpty().WithMessage("Publisher is required")
                .MaximumLength(100).WithMessage("Publisher cannot exceed 100 characters");

            this.RuleFor(e => e.Year)
                .GreaterThan(1450).WithMessage("Year must be after 1450 (printing press invention)")
                .LessThanOrEqualTo(DateTime.Now.Year).WithMessage("Year cannot be in the future");

            this.RuleFor(e => e.EditionNumber)
                .GreaterThan(0).WithMessage("Edition number must be greater than 0");

            this.RuleFor(e => e.PageCount)
                .GreaterThan(0).WithMessage("Page count must be greater than 0");

            this.RuleFor(e => e.BookType)
                .NotEmpty().WithMessage("Book type is required")
                .MaximumLength(50).WithMessage("Book type cannot exceed 50 characters");
        }
    }
}
