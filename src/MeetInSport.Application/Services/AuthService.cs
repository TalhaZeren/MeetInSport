using MeetInSport.Application.DTOs.Auth;
using MeetInSport.Application.Exceptions; // Our custom exceptions!
using MeetInSport.Application.Interface.Repositories;
using MeetInSport.Application.Interface.Services;
using MeetInSport.Domain.Entities;

using BCryptNet = BCrypt.Net.BCrypt; // to prevent the possible conflict.

namespace MeetInSport.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequestDto)
    {
        bool isEmailUnique = await _userRepository.IsEmailUniqueAsync(registerRequestDto.Email);
        if (!isEmailUnique)
        {
            throw new Exception("bu mail ile kayıtlı kullanıcı mevcut.");
        }

        string hashedPassword = BCryptNet.HashPassword(registerRequestDto.Password); // generate a hashed password;

        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Name = registerRequestDto.Name,
            Email = registerRequestDto.Email,
            PasswordHash = hashedPassword, // We stored the hash not the raw password.
            RoleId = registerRequestDto.RoleId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _userRepository.AddAsync(newUser);

        await _userRepository.SaveChangesAsync();

        return new AuthResponseDto
        {
            Id = newUser.Id,
            Name = newUser.Name,
            Email = newUser.Email,
            Message = "Kayıt Başarılı"
        };
    }
}

