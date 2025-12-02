namespace Database_Project.Helpers;

public static class OrderHelper
{
    // Listing orders
    public static async Task ListOrdersAsync()
    {
        await using var db = new ShopContext();
        
        var orders = await db.Orders
            .Include(o => o.Customer)
            .AsNoTracking()
            .OrderBy(c => c.OrderId)
            .ToListAsync();
        
        Console.WriteLine($"\n{"Orders",25}");
        Console.WriteLine($"{"ID",-4} | {"Customer",-25} | {"Date",-12} | {"Total",-12} | {"Status",-12}");
        Console.WriteLine(new string('-', 80));

        foreach (var order in orders)
        {
            Console.WriteLine($"{order.OrderId,-4} | {order.Customer!.CustomerName,-25} | " +
                              $"{order.OrderDate,-12:yyyy-MM-dd} | {order.OrderTotal,-12:C} | " +
                              $"{order.OrderStatus,-12}");
        }
    }

    // Order details
    public static async Task OrderDetailsAsync(int orderId)
    {
        await using var db = new ShopContext();
        
        var order = await db.Orders
            .Include(o => o.OrderRows)
            .ThenInclude(o => o.Product)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        if (order == null)
        {
            Console.WriteLine("Order not found.");
            return;
        }
        
        var orderRows = order.OrderRows.OrderBy(o => o.OrderRowId).ToList();

        if (orderRows.Count == 0)
        {
            Console.WriteLine("Currently no items in this order.");
            return;
        }
        
        Console.WriteLine($"\n{"Order details",40}");
        Console.WriteLine($"{"Product",-35} | {"Price",-10} | {"Quantity",-10} | {"Total",-12}");
        Console.WriteLine(new string('-', 80));

        foreach (var orderRow in orderRows)
        {
            Console.WriteLine($"{orderRow.Product!.ProductName,-35} | {orderRow.Product.PricePerUnit,-10:C} | " +
                              $"{orderRow.OrderRowQuantity,-10} | {orderRow.OrderRowTotal,-12:C}");
        }
        
        Console.WriteLine(new string('-', 80));
        Console.WriteLine($"{"Total",-35} | {"",-10} | {"",-10} | {order.OrderTotal,-12:C}");
    }

    // Adding order
    public static async Task AddOrderAsync()
    {
        await using var db = new ShopContext();

        // Listing customers
        await CustomerHelper.ListCustomersAsync();
        
        // Getting CustomerId using helper method
        var customerId = await IdHelper.GetCustomerId();
        if (customerId == 0)
        {
            return;
        }
        
        // Creating Order (before adding OrderRows to it)
        var order = new Order
        {
            CustomerId = customerId,
            OrderDate = DateTime.Now,
            OrderStatus = "Pending",
            OrderTotal = 0,
            OrderRows = new List<OrderRow>(),
        };
        
        db.Orders.Add(order);
        await db.SaveChangesAsync();
        
        Console.WriteLine("Customer ID found! An order has been created.");
        Console.WriteLine("Let's add some products to your order :)");
        
        // Adding OrderRows to Order
        var done = false;
        
        while (!done)
        {
            // Listing products
            await ProductHelper.ListProductsAsync();

            // Getting product-list for selection logic
            var products = await db.Products
                .Include(p => p.ProductCategory)
                .AsNoTracking()
                .OrderBy(p => p.ProductId)
                .ToListAsync();
            
            // Getting productId using helper method
            var productId = await IdHelper.GetProductId();
            if (productId == 0)
            {
                continue;
            } 
            
            var product = products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
            {
                Console.WriteLine("Product not found.");
                continue;
            }

            Console.Write("Enter quantity: ");
            var quantityInput = Console.ReadLine()?.Trim() ?? string.Empty;
            if (!int.TryParse(quantityInput, out var quantity) || quantity <= 0)
            {
                Console.WriteLine("Invalid quantity.");
                continue;
            }

            if (quantity > product.ProductsInStock)
            {
                Console.WriteLine($"Not enough stock. Only {product.ProductsInStock} left for {product.ProductName}.");
                continue;
            }
            
            // Calculating row total
            var rowTotal = product.PricePerUnit * quantity;
            
            // Adding new OrderRow
            var orderRow = new OrderRow
            {
                OrderId = order.OrderId,
                ProductId = product.ProductId,
                OrderRowQuantity = quantity,
                OrderRowTotal = rowTotal
            };

            db.OrderRows.Add(orderRow);
            
            // Reducing product stock
            product.ProductsInStock -= quantity;
            db.Products.Update(product);
            
            // Updating OrderTotal
            order.OrderTotal += rowTotal;

            Console.WriteLine($"{quantity} × {product.ProductName} added to order (Subtotal: {rowTotal:C}). " +
                              $"Remaining stock: {product.ProductsInStock}");
            
            // Stop loop or repeat to add more OrderRows to Order
            Console.Write("Add another product to your order? (y/n): ");
            var again = Console.ReadLine()?.Trim().ToLower();
            if (again != "y")
            {
                done = true;
            }
        }
        
        // Cancel order if no OrderRows
        if (!order.OrderRows.Any())
        {
            db.Orders.Remove(order);
            await db.SaveChangesAsync();
            Console.WriteLine("Order cancelled: since no products were added.");
            return;
        }
        
        // Save order
        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine($"Order completed! Total: {order.OrderTotal:C}");
            
            await OrderDetailsAsync(order.OrderId);
        }
        catch (Exception ex)
        {
            Console.WriteLine("DB error: " + ex.Message);
        }
    }

    // Delete order
    public static async Task DeleteOrderAsync(int ordId)
    {
        await using var db = new ShopContext();
        var order = await db.Orders.FindAsync(ordId);

        if (order == null)
        {
            Console.WriteLine("Order not found.");
            return;
        }
        
        db.Orders.Remove(order);
        await db.SaveChangesAsync();
        Console.WriteLine("Order deleted!");
    }

    // List orders filtered by status
    public static async Task ListOrdersByStatusAsync(string status)
    {
        await using var db = new ShopContext();
        
        var orders = await db.Orders
            .Include(o => o.Customer)
            .Where(o => o.OrderStatus == status)
            .OrderBy(o => o.OrderId)
            .ToListAsync();

        Console.WriteLine($"\n{"Orders",25}");
        Console.WriteLine($"{"ID",-4} | {"Customer",-25} | {"Date",-12} | {"Total",-12} | {"Status",-12}");
        Console.WriteLine(new string('-', 80));

        foreach (var order in orders)
        {
            Console.WriteLine($"{order.OrderId,-4} | {order.Customer!.CustomerName,-25} | " +
                              $"{order.OrderDate,-12:yyyy-MM-dd} | {order.OrderTotal,-12:C} | {order.OrderStatus,-12}");
        }
    }

    // List orders filtered by customer
    public static async Task ListOrdersByCustomerAsync(int customerId)
    {
        await using var db = new ShopContext();
        var orders = await db.Orders
            .Include(o => o.Customer)
            .Where(o => o.CustomerId == customerId)
            .OrderBy(o => o.OrderId)
            .ToListAsync();

        Console.WriteLine($"\n{"Orders",25}");
        Console.WriteLine($"{"ID",-4} | {"Customer",-25} | {"Date",-12} | {"Total",-12} | {"Status",-12}");
        Console.WriteLine(new string('-', 80));

        foreach (var order in orders)
        {
            Console.WriteLine($"{order.OrderId,-4} | {order.Customer!.CustomerName,-25} | " +
                              $"{order.OrderDate,-12:yyyy-MM-dd} | {order.OrderTotal,-12:C} | {order.OrderStatus,-12}");
        }
    }

    // Paged view of orders
    public static async Task ListOrdersPagedAsync(int page, int pageSize)
    {
        await using var db = new ShopContext();

        var query = db.Orders
            .Include(o => o.Customer)
            .AsNoTracking()
            .OrderBy(o => o.OrderDate);
        
        // Counting orders
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        
        // Get specific page
        var orders = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        Console.WriteLine($"\n{"Orders",25} - Page {page}/{totalPages}, pageSize={pageSize}");
        Console.WriteLine($"{"ID",-4} | {"Customer",-25} | {"Date",-12} | {"Total",-12} | {"Status",-12}");
        Console.WriteLine(new string('-', 80));

        foreach (var order in orders)
        {
            Console.WriteLine($"{order.OrderId,-4} | {order.Customer!.CustomerName,-25} | " +
                              $"{order.OrderDate,-12:yyyy-MM-dd} | {order.OrderTotal,-12:C} | {order.OrderStatus,-12}");
        }
    }
}
