using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Infrastructure.Helpers;

namespace MatchFinder.Application.Services.Impl
{
    public class VerificationService : IVerificationService
    {
        private readonly ICryptographyHelper _cryptographyHelper;
        private readonly IUnitOfWork _unitOfWork;

        public VerificationService(ICryptographyHelper cryptographyHelper, IUnitOfWork unitOfWork)
        {
            _cryptographyHelper = cryptographyHelper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Verification> GenerateTokenAsync(int id)
        {
            var verificationToken = new Verification
            {
                UserId = id,
                TokenSalt = _cryptographyHelper.GenerateSalt(),
                TokenHash = _cryptographyHelper.GenerateHash(Guid.NewGuid().ToString()),
                CreatedAt = DateTime.UtcNow,
                ExpireAt = DateTime.UtcNow.AddMinutes(30)
            };
            await _unitOfWork.VerificationRepository.AddAsync(verificationToken);
            await _unitOfWork.CommitAsync();

            return verificationToken;
        }
    }
}