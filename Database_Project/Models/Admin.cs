namespace Database_Project.Models;

public class Admin
{
    public int AdminId { get; set; }
    
    [Required, MaxLength(50)]
    public string AdminUsername { get; set; } = string.Empty;
    
    [Required]
    public byte[] AdminPasswordHash { get; set; } = Array.Empty<byte>();
    
    [Required]
    public byte[] AdminPasswordSalt { get; set; } = Array.Empty<byte>();
}