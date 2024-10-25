using AutoMapper;
using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Exceptions;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Infrastructure.DataAccess;
using MatchFinder.Infrastructure.Services;

namespace MatchFinder.Application.Services.Impl
{
    public class ImageService : IImageService
    {
        private IMapper _mapper;
        private IFileService _fileService;
        private IUnitOfWork _unitOfWork;
        private MatchFinderContext _context;

        public ImageService(IFileService fileService, IUnitOfWork unitOfWork, MatchFinderContext context, IMapper mapper)
        {
            _fileService = fileService;
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ImageResponse>> UploadAsync(ImageCreateRequest request)
        {
            var fieldExist = await _unitOfWork.FieldRepository.GetAsync(x => x.Id == request.FieldId);
            if (fieldExist == null)
            {
                throw new NotFoundException("Field not found");
            }

            var uploadTasks = request.Images.Where(file => file != null && _fileService.IsImageFile(file))
                          .Select(file => _fileService.SaveFileAsync(file))
                          .ToList();
            if (uploadTasks.Count == 0)
            {
                throw new NotFoundException("No valid images to upload.");
            }
            var filePaths = await Task.WhenAll(uploadTasks);

            var images = new List<Domain.Entities.Image>();

            foreach (var filePath in filePaths)
            {
                var image = new Domain.Entities.Image
                {
                    Url = filePath,
                    Content = null,
                    FieldId = request.FieldId
                };
                await _unitOfWork.ImageRepository.AddAsync(image);
                images.Add(image);
            }

            if (await _unitOfWork.CommitAsync() > 0)
            {
                return _mapper.Map<IEnumerable<ImageResponse>>(images);
            }

            throw new ConflictException("Upload images fail");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var image = await _unitOfWork.ImageRepository.GetAsync(x => x.Id == id);
            if (image == null)
            {
                throw new NotFoundException("Image not found");
            }
            _unitOfWork.ImageRepository.SoftDelete(image);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                return true;
            }
            throw new ConflictException("Failed to delete image");
        }

        public async Task<ImageResponse> GetByIdAsync(int id)
        {
            var image = await _unitOfWork.ImageRepository.GetAsync(x => x.Id == id);
            if (image == null)
            {
                throw new NotFoundException("Image not found");
            }
            return _mapper.Map<ImageResponse>(image);
        }

        public async Task<IEnumerable<ImageResponse>> GetListByFieldIdAsync(int fieldId)
        {
            var images = await _unitOfWork.ImageRepository
                    .GetAllAsync(x => x.FieldId == fieldId);
            return _mapper.Map<IEnumerable<ImageResponse>>(images);
        }
    }
}