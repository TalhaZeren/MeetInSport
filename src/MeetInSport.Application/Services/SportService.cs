using AutoMapper;
using MeetInSport.Application.DTOs.Sports;
using MeetInSport.Application.Interface.Repositories;
using MeetInSport.Application.Interface.Services;
using MeetInSport.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetInSport.Application.Services;

public class SportService : ISportService
{

    private readonly IMapper _mapper;
    private readonly IGenericRepository<Sports> _sportRepository;

    public SportService(IMapper mapper, IGenericRepository<Sports> sportRepository)
    {
        _mapper = mapper;
        _sportRepository = sportRepository;
    }

    public async Task<IEnumerable<SportResponseDto>> GetAllSportsAsync()
    {
        var sports = await _sportRepository.GetAllAsync();
        // Alphabeticially sort the sports by name
        var sortedSports = sports.OrderBy(s => s.Name).ToList();

        return _mapper.Map<IEnumerable<SportResponseDto>>(sortedSports);
    }
}