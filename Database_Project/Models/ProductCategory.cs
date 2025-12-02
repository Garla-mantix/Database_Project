namespace Database_Project.Models;

public class ProductCategory
{
    // PK
    public int ProductCategoryId { get; set; }
    
    public string ProductCategoryName { get; set; }
    
    public string ProductCategoryDescription { get; set; }
    
    // List of products in the category
    public List<Product> Products { get; set; } = new();
    
}