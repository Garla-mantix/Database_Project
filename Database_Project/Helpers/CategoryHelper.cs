namespace Database_Project.Helpers;

public static class CategoryHelper
{
    // Listing categories
    public static async Task ListCategoriesAsync()
    {
        await using var db = new ShopContext();

        var categories = await db.ProductCategories
            .AsNoTracking()
            .OrderBy(pc => pc.ProductCategoryId)
            .ToListAsync();

        if (categories.Count == 0)
        {
            Console.WriteLine("\nNo categories found.");
            return;
        }

        Console.WriteLine($"\n{"Categories",25}");
        Console.WriteLine($"{"ID",-4} | {"Name",-25} | {"Description",-50}");
        Console.WriteLine(new string('-', 85));

        foreach (var category in categories)
        {
            Console.WriteLine($"{category.ProductCategoryId,-4} | {category.ProductCategoryName,-25} | " +
                              $"{category.ProductCategoryDescription,-50}");
        }
    }

    // Adding categories
    public static async Task AddCategoryAsync()
    {
        await using var db = new ShopContext();

        Console.Write("Name: ");
        var categoryName = Console.ReadLine()?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(categoryName) || categoryName.Length > 100)
        {
            Console.WriteLine("Name cannot be empty or more than 100 characters.");
            return;
        }

        var exists = await db.ProductCategories.AnyAsync(c => c.ProductCategoryName == categoryName);
        if (exists)
        {
            Console.WriteLine($"A category named '{categoryName}' already exists.");
            return;
        }

        Console.Write("Description (optional): ");
        var categoryDescription = Console.ReadLine()?.Trim() ?? string.Empty;
        if (categoryDescription.Length > 100)
        {
            Console.WriteLine("Description cannot exceed 100 characters.");
            return;
        }

        db.ProductCategories.Add(new ProductCategory
        {
            ProductCategoryName = categoryName,
            ProductCategoryDescription = string.IsNullOrWhiteSpace(categoryDescription) ? null : categoryDescription
        });

        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Category added successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("DB error: " + ex.Message);
        }
    }

    // Editing categories
    public static async Task EditCategoryAsync(int categoryId)
    {
        await using var db = new ShopContext();

        var category = await db.ProductCategories.FindAsync(categoryId);
        if (category == null)
        {
            Console.WriteLine("Category not found.");
            return;
        }

        Console.Write("New name (leave blank to keep): ");
        var name = Console.ReadLine()?.Trim();
        if (!string.IsNullOrWhiteSpace(name))
        {
            if (name.Length > 100)
            {
                Console.WriteLine("Name cannot exceed 100 characters.");
                return;
            }

            var duplicate = await db.ProductCategories.AnyAsync(c => c.ProductCategoryName == name && c.ProductCategoryId != categoryId);
            if (duplicate)
            {
                Console.WriteLine($"Another category named '{name}' already exists.");
                return;
            }

            category.ProductCategoryName = name;
        }

        Console.Write("New description (leave blank to keep): ");
        var description = Console.ReadLine()?.Trim();
        if (!string.IsNullOrWhiteSpace(description))
        {
            if (description.Length > 100)
            {
                Console.WriteLine("Description cannot exceed 100 characters.");
                return;
            }

            category.ProductCategoryDescription = description;
        }

        await db.SaveChangesAsync();
        Console.WriteLine("Category updated successfully!");
    }

    // Deleting categories
    public static async Task DeleteCategoryAsync(int categoryId)
    {
        await using var db = new ShopContext();

        var category = await db.ProductCategories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.ProductCategoryId == categoryId);

        if (category == null)
        {
            Console.WriteLine("Category not found.");
            return;
        }

        if (category.Products.Any())
        {
            Console.WriteLine($"Cannot delete '{category.ProductCategoryName}'. There are still products in this category.");
            return;
        }

        Console.Write($"Are you sure you want to delete '{category.ProductCategoryName}'? (y/n): ");
        var confirm = Console.ReadLine()?.Trim().ToLower();
        if (confirm != "y")
        {
            Console.WriteLine("Deletion cancelled.");
            return;
        }

        db.ProductCategories.Remove(category);
        await db.SaveChangesAsync();
        Console.WriteLine("Category deleted!");
    }
}
