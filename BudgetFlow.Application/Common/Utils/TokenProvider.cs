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
            new Claim("role",roles.FirstOrDefault())
        };
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

    #region Invitation
    public string GenerateWalletInvitationToken(string email, int walletId)
    {
        string secretKey = configuration["Jwt:Secret"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expirationHours = configuration.GetValue<int>("Jwt:InvitationExpirationInHours");
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("email", email),
                new Claim("walletId", walletId.ToString()),
                new Claim("purpose", "wallet_invitation")
            }),
            Expires = DateTime.UtcNow.AddHours(expirationHours),
            SigningCredentials = credentials,
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"]
        };

        Console.WriteLine("Token Generation Debug Info:");
        Console.WriteLine($"Email: {email}");
        Console.WriteLine($"WalletId: {walletId}");
        Console.WriteLine($"SecretKey: {secretKey}");
        Console.WriteLine($"ExpirationHours: {expirationHours}");
        Console.WriteLine($"Issuer: {configuration["Jwt:Issuer"]}");
        Console.WriteLine($"Audience: {configuration["Jwt:Audience"]}");
        Console.WriteLine($"Expires: {descriptor.Expires}");

        var handler = new JsonWebTokenHandler();
        var token = handler.CreateToken(descriptor);

        Console.WriteLine($"Generated Token: {token}");
        return token;
    }
    public async Task<(bool IsValid, int WalletID, string email)> VerifyWalletInvitationToken(string token)
    {
        Console.WriteLine("Token Verification Debug Info:");
        Console.WriteLine($"Input Token: {token}");

        // Remove Bearer prefix if exists
        if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            token = token.Substring(7);
            Console.WriteLine($"Token after removing Bearer prefix: {token}");
        }

        // URL decode the token
        try
        {
            token = Uri.UnescapeDataString(token);
            Console.WriteLine($"Token after URL decode: {token}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error decoding token: {ex.Message}");
            return (false, 0, null);
        }

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

        Console.WriteLine($"SecretKey: {secretKey}");
        Console.WriteLine($"Issuer: {configuration["Jwt:Issuer"]}");
        Console.WriteLine($"Audience: {configuration["Jwt:Audience"]}");

        try
        {
            var result = handler.ValidateTokenAsync(token, parameters).Result;
            Console.WriteLine($"Token Validation Result: {result.IsValid}");

            if (!result.IsValid)
            {
                Console.WriteLine("Token validation failed");
                return (false, 0, null);
            }

            var claims = result.ClaimsIdentity.Claims.ToList();
            var walletIdClaim = claims.FirstOrDefault(c => c.Type == "walletId");
            var emailClaim = claims.FirstOrDefault(c => c.Type == "email");
            var purposeClaim = claims.FirstOrDefault(c => c.Type == "purpose");

            Console.WriteLine($"WalletId Claim: {walletIdClaim?.Value}");
            Console.WriteLine($"Email Claim: {emailClaim?.Value}");
            Console.WriteLine($"Purpose Claim: {purposeClaim?.Value}");

            if (purposeClaim?.Value != "wallet_invitation")
            {
                Console.WriteLine("Invalid token purpose");
                return (false, 0, null);
            }

            var success = int.TryParse(walletIdClaim.Value, out int walletId);
            Console.WriteLine($"WalletId Parse Success: {success}, Value: {walletId}");

            return (success, walletId, emailClaim.Value);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error validating token: {ex.Message}");
            Console.WriteLine($"Error details: {ex}");
            return (false, 0, null);
        }
    }


    #endregion
}
