using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleConsole.Infrastructure.Models
{
    public class ScheduleTable
    {
        public int Id { get; set; }
        public DateTime ScheduleDateTime { get; set; }
        public string? ScheduleContent { get; set; }
    }
}
