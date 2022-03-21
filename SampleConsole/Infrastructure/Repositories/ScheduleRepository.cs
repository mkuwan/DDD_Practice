using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SampleConsole.Domain.Repositories;
using SampleConsole.Domain.SampleModel;
using SampleConsole.Domain.SeedWork;

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

        public async Task<List<Schedule>> GetAllAsync()
        {
            return await _context.Schedules.ToListAsync();
        }

        public async Task<Schedule> GetByIdAsync(int id)
        {
            return await _context.Schedules.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
