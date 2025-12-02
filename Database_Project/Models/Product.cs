namespace Database_Project.Models;

public class Product
{
    // PK
    public  int ProductId { get; set; }
    
    // FK
    public int? ProductCategoryId { get; set; }
    
    [Required, MaxLength(100)]
    public  string? ProductName { get; set; }
    
    [Required]
    public decimal PricePerUnit { get; set; }
    
    [Required]
    public int ProductsInStock { get; set; }
    
    // Navigation
    public ProductCategory? ProductCategory { get; set; }
}   