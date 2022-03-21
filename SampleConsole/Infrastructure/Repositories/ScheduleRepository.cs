using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SampleConsole.Domain.Repositories;
using SampleConsole.Domain.SampleModel;
using SampleConsole.Domain.SeedWork;
using SampleConsole.Infrastructure.Models;

namespace SampleConsole.Infrastructure.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly SampleDbContext _context;

        public ScheduleRepository(SampleDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<List<Schedule?>> GetAllAsync()
        {
            return ScheduleDto.FromScheduleTableList(await _context.Schedules.ToListAsync());
        }

        public async Task<Schedule?> GetByIdAsync(int id)
        {
            return ScheduleDto.FromScheduleTable(await _context.Schedules.FirstOrDefaultAsync(x => x.Id == id));
        }

        public async Task<Schedule> CreateScheduleAsync(Schedule schedule)
        {

            ScheduleTable scheduleTable = new ScheduleTable()
            {
                ScheduleDateTime = schedule.ScheduleDateTime,
                ScheduleContent = schedule.ScheduleContent
            };
            await _context.Schedules.AddAsync(scheduleTable);
            await _context.SaveChangesAsync();

            var newSchedule = ScheduleDto.FromScheduleTable(scheduleTable);

            return newSchedule;
        }
    }

    public static class ScheduleDto
    {
        public static Schedule FromScheduleTable(ScheduleTable scheduleTable)
        {
            if (scheduleTable == null)
                return null;

            return new Schedule(scheduleTable.Id, scheduleTable.ScheduleDateTime, scheduleTable.ScheduleContent);
        }

        public static ScheduleTable FromSchedule(Schedule schedule)
        {
            if (schedule == null)
                return null;

            return new ScheduleTable()
            {
                Id = schedule.Id, 
                ScheduleDateTime = schedule.ScheduleDateTime,
                ScheduleContent = schedule.ScheduleContent
            };
        }

        public static List<Schedule> FromScheduleTableList(List<ScheduleTable> scheduleTables)
        {
            if (scheduleTables == null) return null;

            List<Schedule> schdules = new();
            foreach (ScheduleTable scheduleTable in scheduleTables)
            {
                schdules.Add(FromScheduleTable(scheduleTable));
            }

            return schdules;
        }
    }

    
}
