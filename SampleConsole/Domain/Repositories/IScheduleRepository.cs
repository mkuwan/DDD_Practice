using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SampleConsole.Domain.SampleModel;
using SampleConsole.Domain.SeedWork;

namespace SampleConsole.Domain.Repositories
{
    public interface IScheduleRepository : IRepository<Schedule>
    {
        Task<List<Schedule>> GetAllAsync();

        Task<Schedule> GetByIdAsync(int id);
    }
}
