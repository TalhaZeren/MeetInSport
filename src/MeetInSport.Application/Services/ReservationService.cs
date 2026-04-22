using AutoMapper;
using MeetInSport.Application.DTOs.Reservation;
using MeetInSport.Application.Exceptions;
using MeetInSport.Application.Interface.Repositories;
using MeetInSport.Application.Interface.Services;
using MeetInSport.Domain.Entities;
using MeetInSport.Domain.Enum;


namespace MeetInSport.Application.Services;

public class ReservationService : IReservationService
{

    private readonly IReservationRepository _reservationRepository;
    private readonly IGenericRepository<LessonPackage> _packageRepository;
    private readonly IMapper _mapper;

    public ReservationService(IReservationRepository reservationRepository, IGenericRepository<LessonPackage> packageRepository, IMapper mapper)
    {
        _reservationRepository = reservationRepository;
        _packageRepository = packageRepository;
        _mapper = mapper;
    }

    public async Task<ReservationResponseDto> CreateReservationAsync(CreateReservationDto createReservationDto, Guid studentId)
    {
        // Verify whether the package is exit or not;
        var requiredPackage = await _packageRepository.GetByIdAsync(createReservationDto.PackageId) ?? throw new NotFoundException(nameof(LessonPackage), createReservationDto.PackageId);

        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            PackageId = requiredPackage.Id,
            CoachId = requiredPackage.CoachId,
            ScheduledAt = createReservationDto.ScheduleAt.ToUniversalTime(),
            LocationType = createReservationDto.LocationType,
            Notes = createReservationDto.Notes,
            Status = ReservationStatus.Pending, // all new reservations start as panding
            CreatedAt = DateTime.UtcNow,
        };
        await _reservationRepository.AddAsync(reservation);
        await _reservationRepository.SaveChangesAsync();
        return _mapper.Map<ReservationResponseDto>(reservation);
    }
}