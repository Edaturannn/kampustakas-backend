using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Takas.Api.Data;
using Takas.Api.DTOs.Auth;
using Takas.Api.Entities;
using Takas.Api.Extensions;
using Takas.Api.Helpers;
using Takas.Api.Services.Interfaces;

namespace Takas.Api.Services.Implementations;

public class AuthService(
    AppDbContext dbContext,
    ICurrentUserAccessor currentUserAccessor,
    IPasswordHasher<User> passwordHasher,
    IConfiguration configuration) : IAuthService
{
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(request.Email);
        var fullName = request.FullName.Trim();

        var emailExists = await dbContext.Users.AnyAsync(x => x.Email == email, cancellationToken);
        if (emailExists)
        {
            throw new ConflictException("Bu e-posta adresi ile kayıtlı bir kullanıcı zaten var.");
        }

        var user = new User
        {
            FullName = fullName,
            Email = email,
            Role = Enums.UserRole.User,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreateAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(request.Email);
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken)
                   ?? throw new NotFoundException("Bu e-posta adresiyle kayıtlı kullanıcı bulunamadı.");

        var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new BadRequestException("Şifre hatalı.");
        }

        if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return CreateAuthResponse(user);
    }

    public Task<AuthUserResponseDto> GetMeAsync(CancellationToken cancellationToken = default)
    {
        var user = currentUserAccessor.GetRequiredCurrentUser();
        return Task.FromResult(user.ToAuthUserResponse());
    }

    private AuthResponseDto CreateAuthResponse(User user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(GetExpireMinutes());
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetRequiredJwtKey())),
            SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt,
            User = user.ToAuthUserResponse()
        };
    }

    private string GetRequiredJwtKey()
    {
        return configuration["Jwt:Key"]
               ?? throw new InvalidOperationException("Jwt:Key ayarı bulunamadı.");
    }

    private int GetExpireMinutes()
    {
        var expireMinutes = configuration.GetValue<int?>("Jwt:ExpireMinutes");
        var resolvedExpireMinutes = expireMinutes.GetValueOrDefault(120);
        return resolvedExpireMinutes > 0 ? resolvedExpireMinutes : 120;
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }
}
