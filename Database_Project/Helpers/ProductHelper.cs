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

        if (products.Count == 0)
        {
            Console.WriteLine("No products found.");
            return;
        }

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
    
    // Adding products
    public static async Task AddProductAsync()
    {
        await using var db = new ShopContext();
    
        Console.Write("Name: ");
        var productName = Console.ReadLine() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(productName) || productName.Length > 100)
        {
            Console.WriteLine("Name cannot be empty or more than 100 characters.");
            return;
        }

        if (await db.Products.AnyAsync(p => p.ProductName == productName))
        {
            Console.WriteLine("A product with this name already exists.");
            return;
        }

        await CategoryHelper.ListCategoriesAsync();
        var categoryId = await IdHelper.GetCategoryId();
        if (categoryId == 0)
        {
            Console.WriteLine("No valid category selected. Product will be uncategorized.");
        }

        Console.Write("Price per unit: ");
        if (!decimal.TryParse(Console.ReadLine(), out var price) || price <= 0)
        {
            Console.WriteLine("Price must be greater than 0.");
            return;
        }

        Console.Write("Quantity in stock: ");
        if (!int.TryParse(Console.ReadLine(), out var stock) || stock < 0)
        {
            Console.WriteLine("Quantity must be greater than or equal to 0.");
            return;
        }

        db.Products.Add(new Product
        {
            ProductName = productName,
            ProductCategoryId = categoryId == 0 ? null : categoryId,
            PricePerUnit = price,
            ProductsInStock = stock
        });
    
        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Product added!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("DB error: " + ex.Message);
        }
    }
    
    // Editing products
    public static async Task EditProductAsync(int productId)
    {
        await using var db = new ShopContext();
        
        var product = await db.Products.FindAsync(productId);

        if (product == null)
        {
            Console.WriteLine("Product not found.");
            return;
        }
        
        Console.Write("New name (leave empty to keep): ");
        var name = Console.ReadLine()?.Trim();
        if (!string.IsNullOrWhiteSpace(name))
        {
            product.ProductName = name;
        }
        
        Console.Write("Change category? (y/n): ");
        var changeCat = Console.ReadLine()?.Trim().ToLower();
        if (changeCat == "y")
        {
            await CategoryHelper.ListCategoriesAsync();
            var categoryId = await IdHelper.GetCategoryId();
            if (categoryId != 0)
            {
                product.ProductCategoryId = categoryId;
            }
        }
        
        Console.Write("New price per unit (leave empty to keep): ");
        var priceInput = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(priceInput) && decimal.TryParse(priceInput, out var price) && price > 0)
        {
            product.PricePerUnit = price;
        }
        else if (!string.IsNullOrEmpty(priceInput))
        {
            Console.WriteLine("Invalid price. Keeping old value.");
        }
        
        Console.Write("New stock (leave empty to keep): ");
        var stockInput = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(stockInput) && int.TryParse(stockInput, out var stock) && stock >= 0)
        {
            product.ProductsInStock = stock;
        }
        else if (!string.IsNullOrEmpty(stockInput))
        {
            Console.WriteLine("Invalid stock. Keeping old value.");
        }
        
        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Product updated!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("DB error: " + ex.Message);
        }
    }
    
    // Deleting products
    public static async Task DeleteProductAsync(int prodId)
    {
        await using var  db = new ShopContext();
        
        var product = await db.Products.FindAsync(prodId);

        if (product == null)
        {
            Console.WriteLine("Product not found.");
            return;
        }

        var hasOrderRows = await db.OrderRows.AnyAsync(or => or.ProductId == prodId);
        if (hasOrderRows)
        {
            Console.WriteLine($"Cannot delete '{product.ProductName}' — it is linked to existing orders.");
            return;
        }
        
        Console.Write($"Are you sure you want to delete '{product.ProductName}'? (y/n): ");
        var confirm = Console.ReadLine()?.Trim().ToLower();
        if (confirm != "y")
        {
            Console.WriteLine("Deletion cancelled.");
            return;
        }
        
        db.Products.Remove(product);
        await db.SaveChangesAsync();
        Console.WriteLine("Product deleted!");
    }
}
