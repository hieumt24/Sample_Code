namespace MatchFinder.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        IVerificationRepository VerificationRepository { get; }
        IPartialFieldRepository PartialFieldRepository { get; }
        IFieldRepository FieldRepository { get; }
        IInactiveTimeRepository InactiveTimeRepository { get; }
        IBookingRepository BookingRepository { get; }
        ITeamRepository TeamRepository { get; }
        IMenuRepository MenuRepository { get; }
        ITransactionRepository TransactionRepository { get; }
        IOpponentFindingRepository OpponentFindingRepository { get; }
        IOpponentFindingRequestRepository OpponentFindingRequestRepository { get; }
        INotificationRepository NotificationRepository { get; }
        INotificationUserRepository NotificationUserRepository { get; }
        IRateRepository RateRepository { get; }
        ISlotRepository SlotRepository { get; }
        IFavoriteFieldRepository FavoriteFieldRepository { get; }
        IReportRepository ReportRepository { get; }
        IImageRepository ImageRepository { get; }
        IBlogPostRepository BlogPostRepository { get; }
        IStaffRepository StaffRepository { get; }

        Task<int> CommitAsync();

        int Commit();
    }
}