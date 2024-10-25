using MatchFinder.Application.Constants;
using MatchFinder.Application.Services;
using MatchFinder.Domain.Entities;
using MatchFinder.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
namespace MatchFinder.WebAPI.Backgrounds
{
    public class BookingStatusUpdater : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public BookingStatusUpdater(IServiceProvider serviceProvider)
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
                    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                    var now = DateTime.Now;
                    var today = DateOnly.FromDateTime(now);
                    var currentTimeInSeconds = now.Hour * 3600 + now.Minute * 60 + now.Second;

                    var bookingsToUpdate = context.Bookings
                        .Where(b => b.Status == "WAITING" &&
                                    (b.Date == today &&
                                    b.StartTime <= currentTimeInSeconds || b.Date < today))
                        .Include(b => b.PartialField.Field)
                        .ToList();

                    foreach (var booking in bookingsToUpdate)
                    {
                        booking.Status = "CANCELED";
                        if (await context.SaveChangesAsync() > 0)
                        {
                            var trans = new MatchFinder.Domain.Entities.Transaction();
                            trans.Status = TransactionStatus.SUCCESSFUL;
                            trans.Type = TransactionType.REFUND;
                            trans.Amount = booking.DepositAmount;
                            trans.Description = $"Hoàn tiền đặt sân do hết hạn tại {booking.PartialField.Name} của {booking.PartialField.Field.Name} vào {booking.Date.ToString("dd/MM/yyyy")} {GetTimeString(booking.StartTime)} - {GetTimeString(booking.EndTime)}";
                            trans.UserId = 1;
                            trans.Booking = booking;
                            trans.ReciverId = booking.UserId;
                            await context.Transactions.AddAsync(trans);
                            await context.SaveChangesAsync();
                            var receiverIds = new List<int> { booking.UserId };

                            var notification = new Notification()
                            {
                                Title = $"Huỷ đặt sân do hết hạn",
                                Content = $"Huỷ đặt đặt sân do hết hạn tại {booking.PartialField.Name} của {booking.PartialField.Field.Name} vào {booking.Date.ToString("dd/MM/yyyy")} {GetTimeString(booking.StartTime)} - {GetTimeString(booking.EndTime)}"
                            };
                            await notificationService.SendNotificationToListUser(receiverIds, notification);
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
        private string GetTimeString(int minutes)
        {
            return TimeOnly.MinValue.AddMinutes(minutes / 60).ToString("HH:mm");
        }
    }
}