using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SampleConsole.Domain.SampleModel;

namespace SampleConsole.Application.Commands
{
    public class CreateScheduleCommand : IRequest<Schedule>
    {
        public int Id { get; set; }
        public DateTime ScheduleDateTime { get; set; }
        public string? ScheduleContent { get; set; }
    }
}
