using AutoMapper;
using FlyFeast.API.Models;
using FlyFeast.API.DTOs;
using FlyFeast.API.DTOs.Payments;
using FlyFeast.API.DTOs.Refunds;
using FlyFeast.API.DTOs.Bookings;
using FlyFeast.API.DTOs.Aircraft_Airport;
using FlyFeast.API.DTOs.Routes;
using FlyFeast.API.DTOs.Schedules;
using FlyFeast.API.DTOs.Seats;
using FlyFeast.API.DTOs.User_Role;

public class MappingProfile : Profile
{
    public MappingProfile()
    {

           //ApplicationUser Mappings

        CreateMap<UserRequestDTO, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<ApplicationUser, UserResponseDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.Passengers,
                opt => opt.MapFrom(src => src.Passenger != null
                    ? new List<Passenger> { src.Passenger }
                    : new List<Passenger>()))
            .ForMember(dest => dest.Roles, opt => opt.Ignore());


        CreateMap<ApplicationUser, UserSummaryDTO>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));


        //Passenger

        CreateMap<Passenger, PassengerDTO>().ReverseMap();
        CreateMap<Passenger, PassengerRequestDTO>().ReverseMap();



        //Aircrafts

        CreateMap<Aircraft, AircraftResponseDTO>().ReverseMap();
        CreateMap<AircraftRequestDTO, Aircraft>().ReverseMap();

  
           //Airports

        CreateMap<Airport, AirportDTO>().ReverseMap();


           //Flight Routes

        CreateMap<FlightRoute, RouteResponseDTO>().ReverseMap();
        CreateMap<RouteRequestDTO, FlightRoute>().ReverseMap();


        // -------------------- SCHEDULES --------------------

        // Entity -> ResponseDTO
        CreateMap<Schedule, ScheduleResponseDTO>()
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.DurationMinutes,
                opt => opt.MapFrom(src => (int)(src.ArrivalTime - src.DepartureTime).TotalMinutes))
            .ForMember(dest => dest.DurationFormatted,
                opt => opt.MapFrom(src => $"{(src.ArrivalTime - src.DepartureTime).Hours}h {(src.ArrivalTime - src.DepartureTime).Minutes}m"));

        // ResponseDTO -> Entity
        CreateMap<ScheduleResponseDTO, Schedule>()
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => Enum.Parse<ScheduleStatus>(src.Status)));

        // RequestDTO -> Entity
        CreateMap<ScheduleRequestDTO, Schedule>()
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => Enum.Parse<ScheduleStatus>(src.Status)));

        // Entity -> RequestDTO
        CreateMap<Schedule, ScheduleRequestDTO>()
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()));

        // Entity -> SummaryDTO
        CreateMap<Schedule, ScheduleSummaryDTO>()
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.DurationMinutes,
                opt => opt.MapFrom(src => (int)(src.ArrivalTime - src.DepartureTime).TotalMinutes))
            .ForMember(dest => dest.DurationFormatted,
                opt => opt.MapFrom(src => $"{(src.ArrivalTime - src.DepartureTime).Hours}h {(src.ArrivalTime - src.DepartureTime).Minutes}m"));

        // SummaryDTO -> Entity
        CreateMap<ScheduleSummaryDTO, Schedule>()
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => Enum.Parse<ScheduleStatus>(src.Status)));



        // Seats
        CreateMap<Seat, SeatResponseDTO>()
            .ForMember(dest => dest.Schedule, opt => opt.MapFrom(src => src.Schedule));

        CreateMap<SeatResponseDTO, Seat>()
            .ForMember(dest => dest.Schedule, opt => opt.Ignore());

        CreateMap<SeatRequestDTO, Seat>().ReverseMap();


        //Bookings & Booking Items

        CreateMap<Booking, BookingResponseDTO>().ReverseMap();
        CreateMap<BookingRequestDTO, Booking>().ReverseMap();
        CreateMap<BookingItem, BookingItemDTO>().ReverseMap();


           //Booking Cancellations

        CreateMap<BookingCancellation, BookingCancellationDTO>().ReverseMap();


           //Payments
  
        CreateMap<PaymentRequestDTO, Payment>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

        CreateMap<Payment, PaymentResponseDTO>().ReverseMap();


           //Refunds
        CreateMap<RefundRequestDTO, Refund>()
            .ForMember(dest => dest.ProcessedById, opt => opt.MapFrom(src => src.ProcessedById));

        CreateMap<Refund, RefundResponseDTO>().ReverseMap();
    }
}
