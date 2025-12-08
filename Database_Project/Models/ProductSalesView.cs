namespace Database_Project.Models;

[Keyless]
public class ProductSalesView
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductCategoryName { get; set; } = string.Empty;
    public decimal PricePerUnit { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalSales { get; set; }
}