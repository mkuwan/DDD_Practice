using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SampleConsole.Domain.SampleModel;

namespace SampleConsole.Application.Queries
{
    internal class GetScheduleQuery : IRequest<List<Schedule>>
    {
    }
}
