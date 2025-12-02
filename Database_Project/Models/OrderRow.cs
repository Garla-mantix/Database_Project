namespace Database_Project.Models;

public class OrderRow
{
    // PK
    public int OrderRowId { get; set; }
    
    // FK
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    
    [Required]
    public int OrderRowQuantity { get; set; }
    
    [Required]
    public decimal OrderRowTotal { get; set; }
    
    // Navigation
    public Order? Order { get; set; }
    public Product? Product { get; set; }
}