using MatchFinder.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace MatchFinder.WebAPI.Backgrounds
{
    public class OpponentFindingOverdueUpdater : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public OpponentFindingOverdueUpdater(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<MatchFinderContext>();

                    var now = DateTime.Now;
                    var today = DateOnly.FromDateTime(now);
                    var currentTimeInSeconds = now.Hour * 3600 + now.Minute * 60 + now.Second;

                    var opponentFindingsToUpdate = context.OpponentFindings
                        .Where(of => !of.IsOverdue &&
                                     ((of.Date == today &&
                                     of.StartTime <= currentTimeInSeconds || of.Date < today))
                                        || (of.Booking.Date == today &&
                                            of.Booking.StartTime <= currentTimeInSeconds || of.Booking.Date < today))
                        .Include(of => of.Booking)
                        .ToList();

                    foreach (var opponentFinding in opponentFindingsToUpdate)
                    {
                        opponentFinding.IsOverdue = true;
                    }

                    await context.SaveChangesAsync();
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}