using System.Collections.Generic;
using System.Threading.Tasks;
using MeetInSport.Application.DTOs.Sports;

namespace MeetInSport.Application.Interface.Services;

public interface ISportService
{

    Task<IEnumerable<SportResponseDto>> GetAllSportsAsync();
}