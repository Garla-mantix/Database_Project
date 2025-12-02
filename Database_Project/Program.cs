// -------------------------------------------- Seed Db ----------------------------------

Console.WriteLine("DB: " + Path.Combine(AppContext.BaseDirectory, "ShopContext.db"));

await SeedDb.SeedAsync();

// -------------------------------------------- CLI ----------------------------------
while (true)
{
    Console.WriteLine("\nWelcome to the EFC Shop!");
    Console.WriteLine("1. Customers");
    Console.WriteLine("2. Orders");
    Console.Write("> ");

    var choice = Console.ReadLine()?.Trim()  ?? string.Empty;

    // MENU
    switch (choice)
    {
        case "1":
            Console.WriteLine("\n1. List all customers");
            Console.WriteLine("2. Add new customer");
            Console.WriteLine("3. Edit existing customer");
            Console.WriteLine("4. Delete existing customer");
            Console.WriteLine("5. Back to main menu");
            
            var customerChoice = Console.ReadLine()?.Trim() ?? string.Empty;
            
            switch (customerChoice)
            {
                case "1":
                    await CustomerHelper.ListCustomersAsync();
                    break;
                case "2":
                    await CustomerHelper.AddCustomerAsync();
                    break;
                case "3":
                    await CustomerHelper.ListCustomersAsync();
                    var customerId = await IdHelper.GetCustomerId();
                    await CustomerHelper.EditCustomerAsync(customerId);
                    break;
                case "4":
                    await CustomerHelper.ListCustomersAsync();
                    var custId = await IdHelper.GetCustomerId();
                    await CustomerHelper.DeleteCustomerAsync(custId); 
                    break;
                case "5":
                    Console.WriteLine("Back to main menu...");
                    break;
                default:
                    Console.WriteLine($"Invalid input: Enter a number between 1 and 5!");
                    break;
            }
            break;
        case "2":
            Console.WriteLine("\n1. List orders");
            Console.WriteLine("2. Show order details for a specific order");
            Console.WriteLine("3. Add new order");
            Console.WriteLine("4. Delete existing order");
            Console.WriteLine("5. Back to main menu");
            
            var orderChoice = Console.ReadLine()?.Trim() ?? string.Empty;

            switch (orderChoice)
            {
                case "1":
                    Console.WriteLine("\n1. List all orders");
                    Console.WriteLine("2. Filter orders by status");
                    Console.WriteLine("3. Filter orders by customer");
                    Console.WriteLine("4. Paginated list of orders");
                    Console.WriteLine("5. Back to main menu");
                    
                    var filterChoice = Console.ReadLine()?.Trim() ?? string.Empty;

                    switch (filterChoice)
                    {
                        case "1":
                            await OrderHelper.ListOrdersAsync();
                            break;
                        case "2":
                            Console.WriteLine("\n1. Pending");
                            Console.WriteLine("2. Paid");
                            Console.WriteLine("3. Shipped");
                            
                            var statusChoice = Console.ReadLine()?.Trim() ?? string.Empty;

                            switch (statusChoice)
                            {
                                case "1":
                                    await OrderHelper.ListOrdersByStatusAsync("Pending");
                                    break;
                                case "2":
                                    await OrderHelper.ListOrdersByStatusAsync("Paid");
                                    break;
                                case "3":
                                    await OrderHelper.ListOrdersByStatusAsync("Shipped");
                                    break;
                                default:
                                    Console.WriteLine("Invalid input: Enter a number between 1 and 3!");
                                    break;
                            }
                            break;
                        case "3":
                            await CustomerHelper.ListCustomersAsync();
                            var cId = await IdHelper.GetCustomerId();
                            await OrderHelper.ListOrdersByCustomerAsync(cId);
                            break;
                        case "4":
                            Console.Write("Enter page: ");
                            var page = int.Parse(Console.ReadLine() ?? string.Empty);
            
                            Console.Write("Enter page size: ");
                            var pageSize = int.Parse(Console.ReadLine() ?? string.Empty);

                            await OrderHelper.ListOrdersPagedAsync(page, pageSize);
                            break;
                        case "5":
                            Console.WriteLine("Back to main menu...");
                            break;
                        default:
                            Console.WriteLine($"Invalid input: Enter a number between 1 and 4!");
                            break;
                    }
                    break;
                case "2":
                    await OrderHelper.ListOrdersAsync();
                    var orderId = await IdHelper.GetOrderId();
                    await OrderHelper.OrderDetailsAsync(orderId);
                    break;
                case "3":
                    await OrderHelper.AddOrderAsync();
                    break;
                case "4":
                    await OrderHelper.ListOrdersAsync();
                    var ordId = await IdHelper.GetOrderId();
                    await OrderHelper.DeleteOrderAsync(ordId); 
                    break;
                case "5":
                    Console.WriteLine("Back to main menu...");
                    break;
                default:
                    Console.WriteLine($"Invalid input: Enter a number between 1 and 5!");
                    break;
            }
            break;
        default:
            Console.WriteLine($"Invalid input: Enter a number between 1 and 2!");
            break;
    }
}
