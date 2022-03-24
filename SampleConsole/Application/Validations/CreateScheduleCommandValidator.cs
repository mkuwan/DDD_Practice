using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SampleConsole.Application.Commands;
using SampleConsole.Domain.Repositories;

namespace SampleConsole.Application.Validations
{
    public class CreateScheduleCommandValidator : AbstractValidator<CreateScheduleCommand>
    {

        public CreateScheduleCommandValidator()
        {
            RuleFor(x => x.ScheduleDateTime)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.ScheduleContent)
                .NotEmpty()
                .NotNull();
        }
    }
}
