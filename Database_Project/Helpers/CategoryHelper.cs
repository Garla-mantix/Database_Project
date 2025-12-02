namespace Database_Project.Helpers;

public static class CategoryHelper
{
    public static async Task ListCategoriesAsync()
    {
        await using var db = new ShopContext();
    
        var categories = await db.ProductCategories
            .AsNoTracking()
            .OrderBy(pc => pc.ProductCategoryId)
            .ToListAsync();
        
        Console.WriteLine($"\n{"Categories", 25}");
        Console.WriteLine($"{"ID",-4} | {"Name",-25} | {"Description",-25}");
        Console.WriteLine(new string('-', 85));

        foreach (var category in categories)
        {
            Console.WriteLine($"{category.ProductCategoryId,-4} | {category.ProductCategoryName,-25} |" +
                              $" {category.ProductCategoryDescription,-25}");
        }
    }
}