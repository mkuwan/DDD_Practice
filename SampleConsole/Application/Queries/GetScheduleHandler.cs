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
    internal class GetScheduleHandler : IRequestHandler<GetScheduleQuery, List<Schedule>>
    {
        private readonly IScheduleRepository _repository;

        public GetScheduleHandler(IScheduleRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Schedule>> Handle(GetScheduleQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllAsync();
        }
    }
}
