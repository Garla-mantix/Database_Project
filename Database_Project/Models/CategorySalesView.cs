namespace Database_Project.Models;

[Keyless]
public class CategorySalesView
{
    public int ProductCategoryId { get; set; }
    public string ProductCategoryName { get; set; } = string.Empty;
    public int TotalQuantity { get; set; }
    public decimal TotalSales { get; set; }
}