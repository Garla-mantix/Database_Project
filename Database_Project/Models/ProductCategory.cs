namespace Database_Project.Models;

public class ProductCategory
{
    // PK
    public int ProductCategoryId { get; set; }
    
    [Required, MaxLength(100)]
    public string ProductCategoryName { get; set; }
    
    [MaxLength(100)]
    public string? ProductCategoryDescription { get; set; }
    
    // List of products in the category
    public List<Product> Products { get; set; } = new();
}