using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Moq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Samples.AutofacSamples
{
    public class BackgroundServiceTest
    {
        [Fact]
        public async void TickerTest()
        {
            // Arrange
            var _tickerService = new TickerService();
            using (var mock = AutoMock.GetLoose(builder => builder.RegisterInstance(_tickerService).As<TickerService>()))
            {
                var backService = mock.Create<SampleBackgroundService>();

                // Act
                await backService.StartAsync(CancellationToken.None);
                await Task.Delay(2500);
                await backService.StopAsync(CancellationToken.None);
                var count = backService.Count;

                // Assertion
                Assert.Equal(2, count);
            }
        }

    }


    internal class SampleBackgroundService : BackgroundService
    {
        private readonly TickerService _tickerService;

        public int Count { get; private set; }

        public SampleBackgroundService(TickerService tickerService)
        {
            Count = 0;

            _tickerService = tickerService;
            _tickerService.Ticked += Ticked;
        }

        private void Ticked(object? sender, TickerEventArgs args)
        {
            Count++;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
                _tickerService.OnEverySecond(this, new TickerEventArgs(TimeOnly.FromDateTime(DateTime.Now)));
            }
        }
    }


    internal class TickerService
    {
        public event EventHandler<TickerEventArgs>? Ticked;

        public void OnEverySecond(object? sender, TickerEventArgs args)
        {
            Ticked?.Invoke(this, args);
        }
    }

    internal class TickerEventArgs
    {
        public TickerEventArgs(TimeOnly time)
        {
            Time = time;
        }

        public TimeOnly Time { get; }
    }
}
