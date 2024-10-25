using MatchFinder.Domain.Interfaces;
using MatchFinder.Infrastructure.DataAccess;
using MatchFinder.Infrastructure.Repositories;

namespace MatchFinder.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MatchFinderContext _context;
        private IUserRepository _userRepository;
        private IRefreshTokenRepository _refreshTokenRepository;
        private IVerificationRepository _verificationRepository;
        private IPartialFieldRepository _partialFieldRepository;
        private IFieldRepository _fieldRepository;
        private IInactiveTimeRepository _inactiveTimeRepository;
        private IBookingRepository _bookingRepository;
        private ITeamRepository _teamRepository;
        private IMenuRepository _menuRepository;
        private ITransactionRepository _transactionRepository;
        private IOpponentFindingRepository _opponentFindingRepository;
        private IOpponentFindingRequestRepository _opponentFindingRequestRepository;
        private INotificationRepository _notificationRepository;
        private INotificationUserRepository _notificationUserRepository;
        private IRateRepository _rateRepository;
        private ISlotRepository _slotRepository;
        private IFavoriteFieldRepository _favoriteFieldRepository;
        private IReportRepository _reportRepository;
        private IImageRepository _imageRepository;
        private IBlogPostRepository _blogPostRepository;
        private IStaffRepository _staffRepository;

        public UnitOfWork(MatchFinderContext context)
        {
            _context = context;
        }

        public IUserRepository UserRepository
            => _userRepository ??= new UserRepository(_context);

        public IRefreshTokenRepository RefreshTokenRepository
            => _refreshTokenRepository ??= new RefreshTokenRepository(_context);

        public IVerificationRepository VerificationRepository
            => _verificationRepository ??= new VerificationRepository(_context);

        public IPartialFieldRepository PartialFieldRepository
            => _partialFieldRepository ??= new PartialFieldRepository(_context);

        public IFieldRepository FieldRepository
            => _fieldRepository ??= new FieldRepository(_context);

        public IInactiveTimeRepository InactiveTimeRepository
            => _inactiveTimeRepository ??= new InactiveTimeRepository(_context);

        public IBookingRepository BookingRepository
            => _bookingRepository ??= new BookingRepository(_context);

        public ITeamRepository TeamRepository
            => _teamRepository ??= new TeamRepository(_context);

        public IMenuRepository MenuRepository
            => _menuRepository ??= new MenuRepository(_context);

        public ITransactionRepository TransactionRepository
            => _transactionRepository ??= new TransactionRepository(_context);

        public IOpponentFindingRepository OpponentFindingRepository
            => _opponentFindingRepository ??= new OpponentFindingRepository(_context);

        public IOpponentFindingRequestRepository OpponentFindingRequestRepository
            => _opponentFindingRequestRepository ??= new OpponentFindingRequestRepository(_context);

        public INotificationRepository NotificationRepository
            => _notificationRepository ??= new NotificationRepository(_context);

        public INotificationUserRepository NotificationUserRepository
            => _notificationUserRepository ??= new NotificationUserRepository(_context);

        public IRateRepository RateRepository
            => _rateRepository ??= new RateRepository(_context);

        public ISlotRepository SlotRepository
            => _slotRepository ??= new SlotRepository(_context);

        public IFavoriteFieldRepository FavoriteFieldRepository
            => _favoriteFieldRepository ??= new FavoriteFieldRepository(_context);

        public IReportRepository ReportRepository
            => _reportRepository ??= new ReportRepository(_context);

        public IImageRepository ImageRepository
            => _imageRepository ??= new ImageRepository(_context);

        public IBlogPostRepository BlogPostRepository
            => _blogPostRepository ??= new BlogPostRepository(_context);

        public IStaffRepository StaffRepository
            => _staffRepository ??= new StaffRepository(_context);

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public int Commit()
        {
            return _context.SaveChanges();
        }
    }
}