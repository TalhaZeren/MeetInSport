using MeetInSport.Application.DTOs.Auth;
using MeetInSport.Application.Exceptions; // Our custom exceptions!
using MeetInSport.Application.Interface.Repositories;
using MeetInSport.Application.Interface.Services;
using MeetInSport.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using BCryptNet = BCrypt.Net.BCrypt; // to prevent the possible conflict.

namespace MeetInSport.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;  // 

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    // Login Process.
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
    {
        var user = await _userRepository.GetUserByEmailAsync(loginRequestDto.Email);

        if (user == null || !BCryptNet.Verify(loginRequestDto.Password, user.PasswordHash))
        {
            throw new Exception("Invalid email or password");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.RoleName)
        };

        var jwtKey = _configuration["JwtSettings:Secret"] ?? throw new Exception("Jwt Secret is missing");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = credential,
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"]
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new LoginResponseDto
        {
            Token = tokenHandler.WriteToken(token),
            UserId = user.Id,
            Name = user.Name,
            Role = user.Role.RoleName
        };
    }
    // Register Process.

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequestDto)
    {
        bool isEmailUnique = await _userRepository.IsEmailUniqueAsync(registerRequestDto.Email);
        if (!isEmailUnique)
        {
            throw new Exception("Email is already in use.");
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

        if(registerRequestDto.RoleId == 2){
            newUser.CoachProfile = new Coach{
                Id = Guid.NewGuid(),
                Sport = string.IsNullOrWhiteSpace(registerRequestDto.Sport) ? "Not Specified" : registerRequestDto.Sport,
                HourlyRate = 0.00m,
                Experience = 0,
                AverageRating = 0.0m,
                IsApproved = false,
                Bio = "Not Specified",
                Location = "Not Specified",
                Iban = "Not Specified"
            };
        };

        await _userRepository.AddAsync(newUser);

        await _userRepository.SaveChangesAsync();

        return new AuthResponseDto
        {
            Id = newUser.Id,
            RoleId = newUser.RoleId,
            Name = newUser.Name,
            Email = newUser.Email,
            Sport = registerRequestDto.Sport,
            Message = "Registeration is successful!"
        };
    }
}

