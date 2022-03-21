using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleConsole.BackgroundTasks
{
    public class TickerBackgroundService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(5000, stoppingToken);
                Console.WriteLine($"TickerBackgroundService: {TimeOnly.FromDateTime(DateTime.Now).ToLongTimeString()}");
            }
        }
    }
}
