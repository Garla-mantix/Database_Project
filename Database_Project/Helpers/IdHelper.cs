namespace Database_Project.Helpers;

public static class IdHelper
{
    /// <summary>
    /// Helper method for getting customerId.
    /// </summary>
    /// <returns>Customer ID.</returns>
    public static async Task<int> GetCustomerId()
    {
        Console.Write("Enter ID of customer: ");
        var customerInput = Console.ReadLine()?.Trim() ?? string.Empty;

        if (!int.TryParse(customerInput, out var customerId))
        {
            Console.WriteLine("Invalid input. Please enter a number.");
            return 0;
        }

        // Check if that CustomerId exists
        await using var db = new ShopContext();
        if (!await db.Customers.AnyAsync(a => a.CustomerId == customerId))
        {
            Console.WriteLine($"No customer found with ID {customerId}.");
            return 0;
        }

        return customerId;
    }

    /// <summary>
    /// Helper method for getting orderId.
    /// </summary>
    /// <returns>Order ID (0 when failing)</returns>
    public static async Task<int> GetOrderId()
    {
        Console.Write("Enter order ID: ");
        var orderInput = Console.ReadLine()?.Trim() ?? string.Empty;

        if (!int.TryParse(orderInput, out var orderId))
        {
            Console.WriteLine("Invalid input. Please enter a number.");
            return 0;
        }

        // Check if that OrderId exists
        await using var db = new ShopContext();
        if (!await db.Orders.AnyAsync(a => a.OrderId == orderId))
        {
            Console.WriteLine($"No order found with ID {orderId}.");
            return 0;
        }

        return orderId;
    }
    
    /// <summary>
    /// Helper method for getting productId.
    /// </summary>
    /// <returns>Product ID (0 when failing).</returns>
    public static async Task<int> GetProductId()
    {
        Console.Write("Enter product ID: ");
        var productInput = Console.ReadLine()?.Trim();
        
        if (!int.TryParse(productInput, out var productId))
        {
            Console.WriteLine("Invalid input. Please enter a number.");
        }
            
        // Check if that ProductId exists
        await using var db = new ShopContext();
        if (!await db.Products.AnyAsync(a => a.ProductId == productId))
        {
            Console.WriteLine($"No product found with ID {productId}.");
            return 0;
        }

        return productId;
    }
    
    /// <summary>
    /// Helper method for getting categoryId.
    /// </summary>
    /// <returns>Category ID (0 when failing).</returns>
    public static async Task<int> GetCategoryId()
    {
        Console.Write("Enter category ID: ");
        var categoryInput = Console.ReadLine()?.Trim();
        
        if (!int.TryParse(categoryInput, out var categoryId))
        {
            Console.WriteLine("Invalid input. Please enter a number.");
        }
            
        // Check if that ProductId exists
        await using var db = new ShopContext();
        if (!await db.ProductCategories.AnyAsync(a => a.ProductCategoryId == categoryId))
        {
            Console.WriteLine($"No product found with ID {categoryId}.");
            return 0;
        }

        return categoryId;
    }
}