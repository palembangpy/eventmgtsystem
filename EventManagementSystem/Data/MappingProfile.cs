using AutoMapper;
using EventManagementSystem.Models.DTO;
using EventManagementSystem.Models.Entities;

namespace EventManagementSystem.Data;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => src.UserType.ToString()))
            .ForMember(dest => dest.TotalCertificates, opt => opt.MapFrom(src => src.Certificates.Count))
            .ForMember(dest => dest.TotalEvents, opt => opt.MapFrom(src => src.EventsAsSpeaker.Count));
        
        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore());
        
        CreateMap<UpdateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<EventSchedule, EventScheduleDto>()
            .ForMember(dest => dest.SpeakerName, opt => opt.MapFrom(src => src.Speaker != null ? src.Speaker.Name : null))
            .ForMember(dest => dest.CurrentParticipants, opt => opt.MapFrom(src => src.Participants.Count(p => p.Status == ParticipantStatus.Attended)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.IsFull, opt => opt.MapFrom(src => src.Participants.Count(p => p.Status == ParticipantStatus.Attended) >= src.MaxParticipants))
            .ForMember(dest => dest.DaysUntilEvent, opt => opt.MapFrom(src => (src.StartDate - DateTime.UtcNow).Days));
        
        CreateMap<CreateEventScheduleDto, EventSchedule>()
            .ForMember(dest => dest.EventScheduleId, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        
        CreateMap<UpdateEventScheduleDto, EventSchedule>()
            .ForMember(dest => dest.EventScheduleId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<Certificate, CertificateDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.EventTitle, opt => opt.MapFrom(src => src.EventSchedule.Title))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}
