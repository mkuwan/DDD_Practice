namespace AngularBackend.Services.Backgrounds
{
    public class TickerBackgroundService:  BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
                Console.WriteLine($"TickerBackgroundService: {TimeOnly.FromDateTime(DateTime.Now).ToLongTimeString()}");
            }
        }
    }
}
