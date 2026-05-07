using MeetInSport.Application.DTOs.Auth;
using MeetInSport.Application.Exceptions;
using MeetInSport.Application.Interface.Repositories;
using MeetInSport.Application.Interface.Services;
using MeetInSport.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using BCryptNet = BCrypt.Net.BCrypt;

namespace MeetInSport.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
    {
        var user = await _userRepository.GetUserByEmailAsync(loginRequestDto.Email);

        if (user == null || string.IsNullOrEmpty(loginRequestDto.Password) || !BCryptNet.Verify(loginRequestDto.Password, user.PasswordHash))
        {
            // Now it safely throws a normal exception, which your middleware will handle gracefully
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
            Role = user.Role.RoleName,
            Email = user.Email
        };
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequestDto)
    {
        bool isEmailUnique = await _userRepository.IsEmailUniqueAsync(registerRequestDto.Email);
        if (!isEmailUnique)
        {
            throw new Exception("Email is already in use.");
        }

        string hashedPassword = BCryptNet.HashPassword(registerRequestDto.Password);

        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Name = registerRequestDto.Name,
            Email = registerRequestDto.Email,
            PasswordHash = hashedPassword,
            RoleId = registerRequestDto.RoleId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        // --- NEW LOGIC FOR SPORT ID ---
        if (registerRequestDto.RoleId == 2)
        {
            // Business Rule: Coaches MUST provide a valid SportId
            if (registerRequestDto.SportId == null || registerRequestDto.SportId == Guid.Empty)
            {
                throw new Exception("A valid Sport is required to register as a Coach.");
            }

            newUser.CoachProfile = new Coach
            {
                Id = Guid.NewGuid(),
                SportId = registerRequestDto.SportId.Value, // Map the new Guid relationship
                HourlyRate = 0.00m,
                Experience = 0,
                AverageRating = 0.0m,
                IsApproved = false,
                Bio = "Not Specified",
                Location = "Not Specified",
                Iban = "Not Specified"
            };
        }
        ;

        await _userRepository.AddAsync(newUser);
        await _userRepository.SaveChangesAsync();

        return new AuthResponseDto
        {
            Id = newUser.Id,
            RoleId = newUser.RoleId,
            Name = newUser.Name,
            Email = newUser.Email,
            // Removed the faulty 'Sport = registerRequestDto.S' mapping
            Message = "Registration is successful!"
        };
    }
}