using BudgetFlow.Application.Common.Interfaces.Services;
using System.Security.Cryptography;

namespace BudgetFlow.Application.Common.Utils;
public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 10000;

    public string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA512, HashSize);

        return $"{Convert.ToBase64String(hash)}-{Convert.ToBase64String(salt)}";
    }
    public bool Verify(string password, string passwordHash)
    {
        string[] parts = passwordHash.Split('-');
        byte[] hash = Convert.FromBase64String(parts[0]);
        byte[] salt = Convert.FromBase64String(parts[1]);

        byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA512, HashSize);

        return CryptographicOperations.FixedTimeEquals(hash, inputHash);
    }
}
