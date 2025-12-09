namespace Database_Project.Helpers;

public static class EncryptionHelper
{
    // Key
    private const byte Key = 0x42; // 66 bytes

    /// <summary>
    /// Encrypts a string
    /// </summary>
    /// <param name="text"></param>
    /// <returns>Encrypted text</returns>
    public static string Encrypt(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }
        
        // Converts text to bytes and store in a new array
        var bytes = System.Text.Encoding.UTF8.GetBytes(text);
        
        // Transforms bytes with XOR
        for (var i = 0; i < bytes.Length; i++)
        {
            bytes[i] = (byte)(bytes[i] ^ Key);
        }
        
        // Converts bytes to Base64 string
        return Convert.ToBase64String(bytes);
    }
    
    /// <summary>
    /// Decrypts a string
    /// </summary>
    /// <param name="encryptedText"></param>
    /// <returns></returns>
    public static string Decrypt(string encryptedText)
    {
        if (string.IsNullOrEmpty(encryptedText))
        {
            return encryptedText;
        }
        
        // Converts Base64 string to bytes again
        var bytes = Convert.FromBase64String(encryptedText);
        
        // Transforms bytes back with XOR
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = (byte)(bytes[i] ^ Key);
        }
        
        // Converts bytes back to string
        return System.Text.Encoding.UTF8.GetString(bytes);
    }
}