using AutoMapper;
using MatchFinder.Application.Constants;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Models;

namespace MatchFinder.Application.Configurations
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserResponse>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<User, CurrentUserResponse>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<PartialField, PartialFieldResponse>()
                .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.Field.Name));

            CreateMap<PartialField, PartialFieldWithNumberBookingsResponse>()
                .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.Field.Name))
                .ForMember(dest => dest.NumberWaiting, opt => opt.MapFrom(src => src.Bookings.Count(b => b.Status == BookingStatus.WAITING)))
                .ForMember(dest => dest.NumberAccepted, opt => opt.MapFrom(src => src.Bookings.Count(b => b.Status == BookingStatus.ACCEPTED)))
                .ForMember(dest => dest.NumberRejected, opt => opt.MapFrom(src => src.Bookings.Count(b => b.Status == BookingStatus.REJECTED)))
                .ForMember(dest => dest.NumberCanceled, opt => opt.MapFrom(src => src.Bookings.Count(b => b.Status == BookingStatus.CANCELED)));

            CreateMap<Field, FieldResponse>()
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner.UserName))
                .ForMember(dest => dest.OpenTime, opt => opt.MapFrom(src => ConvertSecondsToHourMinString(src.OpenTime)))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => Rating(src.Rates)))
                .ForMember(dest => dest.CloseTime, opt => opt.MapFrom(src => ConvertSecondsToHourMinString(src.CloseTime)))
                .ForMember(dest => dest.NumberOfBookings, opt => opt.MapFrom(src =>
                    CalculateTotalBookings(src.PartialFields)));

            CreateMap<Booking, BookingResponse>()
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => ConvertSecondsToHourMinString(src.StartTime)))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => ConvertSecondsToHourMinString(src.EndTime)))
                .ForMember(dest => dest.UserBookingName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.UserBookingPhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.PartialFieldName, opt => opt.MapFrom(src => src.PartialField.Name))
                .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team.Name))
                .ForMember(dest => dest.FieldId, opt => opt.MapFrom(src => src.PartialField.FieldId))
                .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.PartialField.Field.Name))
                .ForMember(dest => dest.FieldAvatar, opt => opt.MapFrom(src => src.PartialField.Field.Avatar))
                .ForMember(dest => dest.FieldStar, opt => opt.MapFrom(src => Rating(src.PartialField.Field.Rates)))
                .ForMember(dest => dest.PostId, opt => opt.MapFrom(src =>
                    src.OpponentFindings.Any() ? src.OpponentFindings.FirstOrDefault().Id : (int?)null))
                .ForMember(dest => dest.IsRated, opt => opt.MapFrom(src => src.Rates.Any()))
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.PartialField.Field.OwnerId))
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.PartialField.Field.Owner.UserName));

            CreateMap<Team, TeamResponse>()
                .ForMember(dest => dest.CaptainName, opt => opt.MapFrom(src => src.Captain.UserName));

            CreateMap<InactiveTime, InactiveTimeResponse>();

            CreateMap<Menu, MenuResponse>()
                .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.Field.Name))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ItemName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ItemDescription));

            CreateMap<Transaction, TransactionResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.ReciverName, opt => opt.MapFrom(src => src.Reciver.UserName));

            CreateMap<OpponentFinding, OpponentFindingResponse>()
                .ForMember(dest => dest.UserFindingName, opt => opt.MapFrom(src => src.UserFinding.UserName))
                .ForMember(dest => dest.UserFindingPhoneNumber, opt => opt.MapFrom(src => src.UserFinding.PhoneNumber))
                .ForMember(dest => dest.UserFindingAvatar, opt => opt.MapFrom(src => src.UserFinding.Avatar))
                .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.Field != null ? src.Field.Name : src.FieldName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Field != null ? src.Field.Address : src.FieldAddress))
                .ForMember(dest => dest.Province, opt => opt.MapFrom(src => src.Field != null ? src.Field.Province : src.FieldProvince))
                .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.Field != null ? src.Field.District : src.FieldDistrict))
                .ForMember(dest => dest.Commune, opt => opt.MapFrom(src => src.Field != null ? src.Field.Commune : src.FieldCommune))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Booking != null ? src.Booking.Date : src.Date))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.Booking != null ? ConvertSecondsToHourMinString(src.Booking.StartTime) : ConvertSecondsToHourMinString(src.StartTime.Value)))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.Booking != null ? ConvertSecondsToHourMinString(src.Booking.EndTime) : ConvertSecondsToHourMinString(src.EndTime.Value)))
                .ForMember(dest => dest.TotalRequest, opt => opt.MapFrom(src => src.OpponentFindingRequests.Count));

            CreateMap<OpponentFindingRequest, OpponentFindingResponseWithUserRequesting>()
                .ForMember(dest => dest.UserRequestingName, opt => opt.MapFrom(src => src.UserRequesting.UserName))
                .ForMember(dest => dest.UserRequestingPhoneNumber, opt => opt.MapFrom(src => src.UserRequesting.PhoneNumber))
                .ForMember(dest => dest.UserRequestingAvatar, opt => opt.MapFrom(src => src.UserRequesting.Avatar));

            CreateMap<NotificationUser, NotificationResponse>()
                 .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Notification.CreatedAt))
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Notification.Id))
                 .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Notification.Title))
                 .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Notification.Content));

            CreateMap<Rate, RateResponse>()
                .ForMember(dest => dest.RaterName, opt => opt.MapFrom(src => src.Rater.UserName))
                .ForMember(dest => dest.RaterAvatar, opt => opt.MapFrom(src => src.Rater.Avatar))
                .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.Field.Name));

            CreateMap<Slot, SlotResponse>()
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => ConvertSecondsToHourMinString(src.StartTime)))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => ConvertSecondsToHourMinString(src.EndTime)))
                .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.Field.Name));

            CreateMap<Report, ReportResponse>()
                .ForMember(dest => dest.ReportType, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.UserCreatedId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.UserCreated, opt => opt.MapFrom(src => src.Reporter.UserName));

            CreateMap<Image, ImageResponse>();

            CreateMap<BlogPost, BlogPostResponse>();

            CreateMap<Staff, StaffResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.Field.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PhoneNumer, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.User.Avatar))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.User.Status.ToString()));
        }

        private int CalculateTotalBookings(IEnumerable<PartialField> partialFields)
        {
            return partialFields.Sum(pf => pf.Bookings?.Count ?? 0);
        }

        private string Rating(ICollection<Rate> rates)
        {
            if (rates == null || !rates.Any())
            {
                return null;
            }

            double averageRating = rates.Average(r => r.Star);
            return Math.Round(averageRating, 1).ToString("0.0");
        }

        private string ConvertSecondsToTimeString(int totalSeconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
            return time.ToString(@"hh\:mm\:ss");
        }

        private string ConvertSecondsToHourMinString(int totalSeconds)
        {
            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            return $"{hours:D2}:{minutes:D2}";
        }
    }
}