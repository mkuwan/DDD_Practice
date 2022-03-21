using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SampleConsole.Domain.Repositories;
using SampleConsole.Domain.SampleModel;

namespace SampleConsole.Application.Commands
{
    internal class CreateScheduleHandler : IRequestHandler<CreateScheduleCommand, Schedule>
    {
        private readonly IScheduleRepository _scheduleRepository;

        public CreateScheduleHandler(IScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }

        public async Task<Schedule> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
        {
            Schedule schedule = new Schedule(0, request.ScheduleDateTime, request.ScheduleContent);
            return await _scheduleRepository.CreateScheduleAsync(schedule);
        }
    }
}
