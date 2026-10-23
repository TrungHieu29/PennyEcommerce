using System.Security.Cryptography;

namespace PennyEcommerce.Services.Auth;

public class PasswordService
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 600_000;

    public (string Hash, string Salt, int Iterations) HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize
        );

        return (
            Convert.ToBase64String(hash),
            Convert.ToBase64String(salt),
            Iterations
        );
    }

    public bool VerifyPassword(string password, string savedHash, string savedSalt, int savedIterations)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(savedHash) || string.IsNullOrWhiteSpace(savedSalt))
        {
            return false;
        }

        byte[] salt = Convert.FromBase64String(savedSalt);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            savedIterations,
            HashAlgorithmName.SHA256,
            HashSize
        );

        return CryptographicOperations.FixedTimeEquals(hash, Convert.FromBase64String(savedHash));
    }
}