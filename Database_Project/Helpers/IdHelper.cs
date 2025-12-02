namespace Database_Project.Helpers;

public static class IdHelper
{
    // Helper method for getting customerId
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

    // Helper method for getting orderId
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
    
    // Helper method for getting productId
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
}