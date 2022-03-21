using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SampleConsole.Domain.Repositories;
using SampleConsole.Domain.SampleModel;
using SampleConsole.Infrastructure;

namespace SampleConsole.Application.Queries
{
    internal class GetScheduleByIdHandler : IRequestHandler<GetScheduleByIdQuery, Schedule>
    {
        private readonly IScheduleRepository _repository;

        public GetScheduleByIdHandler(IScheduleRepository repository)
        {
            _repository = repository;
        }

        public async Task<Schedule?> Handle(GetScheduleByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetByIdAsync(request.Id);
        }
    }
}
