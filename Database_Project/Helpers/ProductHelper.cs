namespace Database_Project.Helpers;

public static class ProductHelper
{
    // Listing products
    public static async Task ListProductsAsync()
    {
        await using var db = new ShopContext();
        
        var products = await db.Products
            .Include(p => p.ProductCategory)
            .AsNoTracking()
            .OrderBy(p => p.ProductId)
            .ToListAsync();

        Console.WriteLine("\nProducts:");
        Console.WriteLine($"{"ID",-4} | {"Name",-35} | {"Category",-15} | {"Price",-10} | {"Stock",-10}");
        Console.WriteLine(new string('-', 90));

        foreach (var p in products)
        {
            var category = p.ProductCategory?.ProductCategoryName ?? "Uncategorized";
            Console.WriteLine($"{p.ProductId,-4} | {p.ProductName,-35} | {category,-15} | " +
                              $"{p.PricePerUnit,-10:C} | {p.ProductsInStock,-10}");
        }
    }
    
    
}