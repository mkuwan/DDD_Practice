using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SampleConsole.Domain.SampleModel;

namespace SampleConsole.Application.Queries
{
    internal class GetScheduleByIdQuery : IRequest<Schedule>
    {
        public GetScheduleByIdQuery(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }
}
