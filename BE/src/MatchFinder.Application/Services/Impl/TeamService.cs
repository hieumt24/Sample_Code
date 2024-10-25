using AutoMapper;
using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Exceptions;
using MatchFinder.Domain.Interfaces;

namespace MatchFinder.Application.Services.Impl
{
    public class TeamService : ITeamService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public TeamService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<TeamResponse> CreateTeamAsync(TeamCreateRequest request, int uid)
        {
            var leader = await _unitOfWork.UserRepository.GetAsync(x => x.Id == uid);
            if (leader == null)
            {
                throw new ConflictException("Account invalid!");
            }

            var newTeam = new Team
            {
                Name = request.Name,
                Captain = leader
            };

            await _unitOfWork.TeamRepository.AddAsync(newTeam);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                return _mapper.Map<TeamResponse>(newTeam);
            }
            throw new ConflictException("Create team fail");
        }
    }
}