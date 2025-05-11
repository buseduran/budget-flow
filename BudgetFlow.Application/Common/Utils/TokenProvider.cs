using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BudgetFlow.Application.Common.Utils;
public sealed class TokenProvider(IConfiguration configuration, IUserRepository userRepository) : ITokenProvider
{
    public async Task<string> Create(User user)
    {
        string secretKey = configuration["Jwt:Secret"];
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        #region Get roles and append them to Claims
        var roles = await userRepository.GetUserRolesAsync(user.ID);
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.ID.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.Name),
            new Claim(JwtRegisteredClaimNames.Birthdate,user.CreatedAt.ToString()),
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        #endregion

        var minutes = configuration.GetValue<int>("Jwt:ExpirationInMinutes");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(minutes),
            SigningCredentials = credentials,
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"]
        };

        var handler = new JsonWebTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);
        return token.ToString();
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }

    #region Password Reset
    public string GeneratePasswordResetToken(User user)
    {
        string secretKey = configuration["Jwt:Secret"];
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var expirationMinutes = configuration.GetValue<int>("Jwt:PasswordResetExpirationInMinutes");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.ID.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("purpose", "reset_password")
        }),
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            SigningCredentials = credentials,
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"]
        };

        var handler = new JsonWebTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);
        return token.ToString();
    }

    public bool VerifyPasswordResetToken(int userId, string token)
    {
        var handler = new JsonWebTokenHandler();
        var secretKey = configuration["Jwt:Secret"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = key,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        var result = handler.ValidateTokenAsync(token, validationParameters).Result;
        if (!result.IsValid)
            return false;

        var claims = result.ClaimsIdentity.Claims;
        var subClaim = claims.FirstOrDefault(c => c.Type == "sub");

        return subClaim?.Value == userId.ToString();
    }
    #endregion

    #region E-mail Confirmation
    public string GenerateEmailConfirmationToken(User user)
    {
        string secretKey = configuration["Jwt:Secret"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


        var expirationMinutes = configuration.GetValue<int>("Jwt:EmailConfirmExpirationInMinutes");
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.ID.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("purpose", "email_confirmation")
        }),
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            SigningCredentials = credentials,
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"]
        };

        var handler = new JsonWebTokenHandler();
        return handler.CreateToken(descriptor);
    }
    public bool VerifyEmailConfirmationToken(int userId, string token)
    {
        var handler = new JsonWebTokenHandler();
        var secretKey = configuration["Jwt:Secret"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = key,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        var result = handler.ValidateTokenAsync(token, parameters).Result;
        if (!result.IsValid)
            return false;

        var claims = result.ClaimsIdentity.Claims.ToList();

        var subClaim = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
        var purposeClaim = claims.FirstOrDefault(c => c.Type == "purpose");

        return subClaim?.Value == userId.ToString() && purposeClaim?.Value == "email_confirmation";
    }

    #endregion
}
