namespace Database_Project.Helpers;

public static class CategoryHelper
{
    /// <summary>
    /// Lists categories.
    /// </summary>
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

   /// <summary>
   /// Adds a new category.
   /// </summary>
    public static async Task AddCategoryAsync()
    {
        await using var db = new ShopContext();

        Console.Write("Name: ");
        var categoryName = (Console.ReadLine() ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(categoryName) || categoryName.Length > 100)
        {
            Console.WriteLine("Name cannot be empty or more than 100 characters.");
            return;
        }

        // Fetch all categories to check if the name already exists
        var exists = await db.ProductCategories.AnyAsync(c => c.ProductCategoryName == categoryName);
        if (exists)
        {
            Console.WriteLine($"A category named '{categoryName}' already exists.");
            return;
        }

        Console.Write("Description (optional): ");
        var categoryDescription = (Console.ReadLine() ?? string.Empty).Trim();
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

    /// <summary>
    /// Edits a category.
    /// </summary>
    /// <param name="categoryId">ID of category to be edited</param>
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

    /// <summary>
    /// Deletes a category.
    /// </summary>
    /// <param name="categoryId">ID of category to be deleted.</param>
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
    
    /// <summary>
    /// VIEW - lists sales by category.
    /// </summary>
    public static async Task ListCategorySalesAsync()
    {
        await using var db = new ShopContext();

        var sales = await db.Set<CategorySalesView>()
            .AsNoTracking()
            .OrderByDescending(cs => (double)cs.TotalSales)
            .ToListAsync();

        Console.WriteLine($"\n{"Category Sales Report",35}");
        Console.WriteLine($"{"Category",-30} | {"Sold",-5} | {"Revenue",-12}");
        Console.WriteLine(new string('-', 55));

        foreach (var cs in sales)
        {
            Console.WriteLine($"{cs.ProductCategoryName,-30} | {cs.TotalQuantity,-5} | {cs.TotalSales,-12:C}");
        }
    }
}
