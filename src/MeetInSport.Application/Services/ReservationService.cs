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
    private readonly ICoachRepository _coachRepository;

    public ReservationService(IReservationRepository reservationRepository, IGenericRepository<LessonPackage> packageRepository, IMapper mapper, ICoachRepository coachRepository)
    {
        _reservationRepository = reservationRepository;
        _packageRepository = packageRepository;
        _mapper = mapper;
        _coachRepository = coachRepository;
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

    public async Task<IEnumerable<ReservationResponseDto>> GetMyReservationsAsync(Guid userId, string role)
    {
        IReadOnlyList<Reservation> reservations;

        if (role == "Coach")
        {
            // CoachId was found first
            var coach = await _coachRepository.GetCoachByUserIdAsync(userId) ?? throw new NotFoundException("Coach Profile,", userId);
            reservations = await _reservationRepository.GetReservationsByCoachIdAsync(coach.Id);
        }
        else
        {
            reservations = await _reservationRepository.GetReservationsByUserIdAsync(userId);
        }
        return _mapper.Map<IEnumerable<ReservationResponseDto>>(reservations);
    }
}