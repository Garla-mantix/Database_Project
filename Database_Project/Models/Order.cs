namespace Database_Project.Models;

public class Order
{
    // PK
    public int OrderId { get; set; }
    
    // FK
    public int CustomerId { get; set; }

    [Required]
    public DateTime OrderDate { get; set; }
    
    [MaxLength(100)]
    public string? OrderStatus { get; set; }
    
    [Required]
    public decimal OrderTotal { get; set; }
    
    // List of OrderRows
    public List<OrderRow> OrderRows { get; set; } = new();
    
    // Navigation
    public Customer? Customer { get; set; }
}