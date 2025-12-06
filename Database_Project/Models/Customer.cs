namespace Database_Project.Models;

public class Customer
{
    // PK
    public int CustomerId { get; set; }
    
    [Required, MaxLength(100)]
    public string? CustomerName { get; set; }
    
    [Required, MaxLength(100)]
    public string? CustomerEmail { get; set; }
    
    [MaxLength(100)]
    public string? CustomerCity { get; set; }
    
    // Lista av Order
    public List<Order> Orders { get; set; } = new();
}