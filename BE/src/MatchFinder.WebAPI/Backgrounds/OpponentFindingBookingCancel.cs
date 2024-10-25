using MatchFinder.Application.Constants;
using MatchFinder.Domain.Constants;
using MatchFinder.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace MatchFinder.WebAPI.Backgrounds
{
    public class OpponentFindingBookingCancel : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public OpponentFindingBookingCancel(IServiceProvider serviceProvider)
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

                    var cancelBooking = context.Bookings
                        .Where(b => b.Status != BookingStatus.ACCEPTED && b.Status != BookingStatus.WAITING)
                        .Include(b => b.OpponentFindings)
                        .ThenInclude(of => of.OpponentFindingRequests);

                    foreach (var booking in cancelBooking)
                    {
                        foreach (var opponentFinding in booking.OpponentFindings)
                        {
                            foreach (var opponentFindingRequest in opponentFinding.OpponentFindingRequests)
                            {
                                opponentFindingRequest.Status = OpponentFindingRequestStatus.CANCELLED;
                                context.OpponentFindingRequests.Update(opponentFindingRequest);
                            }
                            opponentFinding.Status = OpponentFindingStatus.CANCELLED;
                            context.OpponentFindings.Update(opponentFinding);
                        }
                        await context.SaveChangesAsync();
                    }

                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }
    }
}