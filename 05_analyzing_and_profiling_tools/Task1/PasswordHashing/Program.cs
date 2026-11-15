using PasswordHashing;

byte[] salt = [ 0x3A, 0x9F, 0x01, 0xE2, 0x7C, 0x10, 0x55, 0xB9, 0xD0, 0x44, 0x29, 0x8A, 0xFE, 0x6C, 0x12, 0x00 ];

string hashedPassword = PasswordHashGenerator.GeneratePasswordHashUsingSalt("Password", salt);

Console.WriteLine($"Hashed password: {hashedPassword}");

string hashedPasswordOptimised = PasswordHashGenerator.GeneratePasswordHashUsingSaltOptimised("Password", salt);

Console.WriteLine($"Hashed password: {hashedPasswordOptimised}");