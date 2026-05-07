using System;

namespace MeetInSport.Application.DTOs.Sports;

public class SportResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}