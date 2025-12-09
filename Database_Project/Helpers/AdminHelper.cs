namespace Database_Project.Helpers;

public static class AdminHelper
{
    /// <summary>
    /// Logs in admin.
    /// </summary>
    /// <param name="maxAttempts">Number of attempts before returning false.</param>
    /// <returns>True if authenticated.</returns>
    public static async Task<bool> TryLoginAsync(int maxAttempts = 3)
    {
        await using var db = new ShopContext();

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            Console.WriteLine("\n---- Admin Login ----");
            Console.Write("Username: ");
            var username = Console.ReadLine() ?? string.Empty;

            // Fetch admin from DB and check if username exists
            var admin = await db.Admins.FirstOrDefaultAsync(a => a.AdminUsername == username);
            if (admin == null)
            {
                Console.WriteLine("Invalid username.");
            }
            else
            {
                Console.Write("Password: ");
                var password = ReadPassword();

                // Validate password
                if (VerifyPasswordHash(password, admin.AdminPasswordHash, admin.AdminPasswordSalt))
                {
                    Console.WriteLine($"\nWelcome, {username}! Access granted.");
                    return true;
                }

                Console.WriteLine("Invalid password.");
            }

            if (attempt < maxAttempts)
                Console.WriteLine($"Attempt {attempt}/{maxAttempts} failed. Try again.\n");
        }

        Console.WriteLine("Maximum login attempts reached. Access denied.");
        return false;
    }
    
    /// <summary>
    /// Verifies an entered password against the stored hash and salt.
    /// </summary>
    private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
    {
        // Using the stored salt from the db for hashing
        using var hmac = new HMACSHA512(storedSalt);
        
        // Hashing the entered password
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        
        // Comparing hashed password with stored hash from the db
        return computedHash.SequenceEqual(storedHash);
    }
    
    /// <summary>
    /// Masks password-input with asterisks.
    /// </summary>
    /// <returns>Password.</returns>
    private static string ReadPassword()
    {
        var password = new StringBuilder();
        ConsoleKeyInfo key;

        do
        {
            // Reads keypress without showing it on the screen
            key = Console.ReadKey(true);

            // Backspace removes last character entered (if length > 0)
            if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password.Remove(password.Length - 1, 1);
                Console.Write("\b \b");
            }
            // Else character is appended to the password and asterisk is shown
            else if (!char.IsControl(key.KeyChar))
            {
                password.Append(key.KeyChar);
                Console.Write("*");
            }
            
        } while (key.Key != ConsoleKey.Enter); // Loop until Enter is pressed

        Console.WriteLine();
        return password.ToString();
    }
}