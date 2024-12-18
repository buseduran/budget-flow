using BudgetFlow.Application.Common.Interfaces.Services;
using System.Security.Cryptography;

namespace BudgetFlow.Application.Common.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 10000;

        private readonly HashAlgorithmName algorithmName = HashAlgorithmName.SHA512;

        public string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, algorithmName, HashSize);

            return $"{Convert.ToBase64String(salt)}-{Convert.ToBase64String(hash)}";
        }
    }
}
