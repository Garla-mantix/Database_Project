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
            .OrderByDescending(c => c.OrderDate)
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
            .ThenInclude(p => p!.ProductCategory)
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
        Console.WriteLine($"{"Product",-35} | {"Category",-15} | {"Price",-10} | {"Quantity",-10} | {"Total",-12}");
        Console.WriteLine(new string('-', 90));

        foreach (var orderRow in orderRows)
        {
            var categoryName = orderRow.Product!.ProductCategory?.ProductCategoryName ?? "Uncategorized";
            Console.WriteLine($"{orderRow.Product.ProductName,-35} | {categoryName,-15} | " +
                              $"{orderRow.Product.PricePerUnit,-10:C} | {orderRow.OrderRowQuantity,-10} | " +
                              $"{orderRow.OrderRowTotal,-12:C}");
        }
        
        Console.WriteLine(new string('-', 90));
        Console.WriteLine($"{"Total",-35} | {"",-15} | {"",-10} | {"",-10} | {order.OrderTotal,-12:C}");
    }

    // Adding order
    public static async Task AddOrderAsync()
    {
        await using var db = new ShopContext();

        // Begin transaction
        await using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            // Listing customers
            await CustomerHelper.ListCustomersAsync();

            // Getting CustomerId
            var customerId = await IdHelper.GetCustomerId();
            if (customerId == 0) return;

            // Creating order
            var order = new Order
            {
                CustomerId = customerId,
                OrderDate = DateTime.Now,
                OrderStatus = "Pending",
                OrderTotal = 0,
                OrderRows = new List<OrderRow>()
            };

            db.Orders.Add(order);
            await db.SaveChangesAsync();

            Console.WriteLine("Customer found! An order has been created.");
            Console.WriteLine("Let's add some products to your order :)");

            var done = false;

            while (!done)
            {
                // Display products
                await ProductHelper.ListProductsAsync();

                // Get product ID
                var productId = await IdHelper.GetProductId();
                if (productId == 0)
                {
                    continue;
                }

                // Fetch the selected product
                var product = await db.Products
                    .Include(p => p.ProductCategory)
                    .FirstOrDefaultAsync(p => p.ProductId == productId);

                if (product == null)
                {
                    Console.WriteLine("Product not found.");
                    continue;
                }
                
                // Check if product is available
                if (product.ProductsInStock <= 0)
                {
                    Console.WriteLine($"Cannot add '{product.ProductName}' — no stock available.");
                    continue;
                }

                // Quantity input loop
                int quantity;
                while (true)
                {
                    Console.Write("Enter quantity: ");
                    var input = (Console.ReadLine() ?? string.Empty).Trim();
                    if (!int.TryParse(input, out quantity) || quantity <= 0)
                    {
                        Console.WriteLine("Invalid quantity. Enter a positive number.");
                        continue;
                    }

                    if (quantity > product.ProductsInStock)
                    {
                        Console.WriteLine($"Not enough stock. Only {product.ProductsInStock} left.");
                        continue;
                    }

                    break;
                }

                // Calculate row total
                var rowTotal = product.PricePerUnit * quantity;

                // Add order row
                var orderRow = new OrderRow
                {
                    OrderId = order.OrderId,
                    ProductId = product.ProductId,
                    OrderRowQuantity = quantity,
                    OrderRowTotal = rowTotal
                };
                db.OrderRows.Add(orderRow);

                // Reduce stock
                product.ProductsInStock -= quantity;
                db.Products.Update(product);

                // Update order total
                order.OrderTotal += rowTotal;

                Console.WriteLine($"{quantity} × {product.ProductName} added (Subtotal: {rowTotal:C}). " +
                                  $"Remaining stock: {product.ProductsInStock}");

                // Ask if user wants to add another product
                Console.Write("Add another product? (y/n): ");
                var again = (Console.ReadLine() ?? string.Empty).Trim().ToLower();
                done = again != "y";
            }

            // Cancel order if no products were added
            if (!order.OrderRows.Any())
            {
                await transaction.RollbackAsync();
                Console.WriteLine("Order cancelled: no products were added.");
                return;
            }

            // Save and commit
            await db.SaveChangesAsync();
            await transaction.CommitAsync();

            Console.WriteLine($"Order completed! Total: {order.OrderTotal:C}");
            await OrderDetailsAsync(order.OrderId);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
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
        
        Console.Write($"Are you sure you want to delete? (y/n): ");
        var confirm = Console.ReadLine()?.Trim().ToLower();
        if (confirm != "y")
        {
            Console.WriteLine("Deletion cancelled.");
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
