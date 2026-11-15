using System.Security.Cryptography;

namespace Task1;

public static class PasswordHashGenerator
{
    private const int Iterate = 10000;
    private const int SaltSize = 16;
    private const int DerivedSize = 20;

    public static string GeneratePasswordHashUsingSalt(string passwordText, byte[] salt)
    {
        if (salt.Length < SaltSize)
        {
            throw new ArgumentOutOfRangeException(nameof(salt), $"Salt must be at least {SaltSize} bytes.");
        }

        Span<byte> hashBytes = stackalloc byte[SaltSize + DerivedSize];
        Span<byte> destination = hashBytes.Slice(SaltSize, DerivedSize);

        ReadOnlySpan<byte> saltSpan = salt.AsSpan(0, SaltSize);

        saltSpan.CopyTo(hashBytes[..SaltSize]);
        Rfc2898DeriveBytes.Pbkdf2(passwordText, saltSpan, destination, Iterate, HashAlgorithmName.SHA1);

        var passwordHash = Convert.ToBase64String(hashBytes);

        return passwordHash;
    }
}
