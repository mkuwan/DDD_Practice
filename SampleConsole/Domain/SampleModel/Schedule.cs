using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SampleConsole.Domain.SeedWork;

namespace SampleConsole.Domain.SampleModel
{
    public class Schedule : Entity, IAggregateRoot
    {
        public int ScheduleId => Id;
        public DateTime ScheduleDateTime { get; set; }
        public string? ScheduleContent { get; set; }

        public Schedule(int id, DateTime scheduleDateTime, string? scheduleContent)
        {
            Id = id;
            ScheduleDateTime = scheduleDateTime;
            ScheduleContent = scheduleContent;
        }

        protected Schedule()
        {

        }

        public static Schedule NewSchedule()
        {
            var schedule = new Schedule();
            return schedule;
        }


        public void SetScheduleDate(DateTime dateTime)
        {
            ScheduleDateTime = dateTime;
        }

        public void SetScheduleContent(string? content)
        {
            ScheduleContent = content;
        }
    }
}
