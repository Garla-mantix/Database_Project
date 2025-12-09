// -------------------------------------------- Seed Db ----------------------------------

Console.WriteLine("DB: " + Path.Combine(AppContext.BaseDirectory, "ShopContext.db"));
await SeedDb.SeedAsync();

// -------------------------------------------- ADMIN LOGIN--------------------------------

var loginSuccess = await AdminHelper.TryLoginAsync();
if (!loginSuccess)
{
    Console.WriteLine("Exiting...");
    return;
}

// -------------------------------------------- MAIN CLI ----------------------------------

while (true)
{
    Console.WriteLine("\n ---- Welcome to the EFC Shop! ----");
    Console.WriteLine("1. Customers");
    Console.WriteLine("2. Orders");
    Console.WriteLine("3. Products");
    Console.WriteLine("4. Categories");
    Console.WriteLine("5. Exit");
    Console.Write("> ");

    var choice = Console.ReadLine()?.Trim() ?? string.Empty;

    switch (choice)
    {
        case "1":
            await CustomerMenuAsync();
            break;
        case "2":
            await OrderMenuAsync();
            break;
        case "3":
            await ProductMenuAsync();
            break;
        case "4":
            await CategoryMenuAsync();
            break;
        case "5":
            Console.WriteLine("Exiting...");
            return;
        default:
            Console.WriteLine($"Invalid input: Enter a number between 1 and 5!");
            break;
    }
}
// -------------------------------------------- Submenus ----------------------------------
// ------------------------------ Customer Menu
static async Task CustomerMenuAsync()
{
    Console.WriteLine("\n ---- Customer Menu ----");
    Console.WriteLine("1. List all customers");
    Console.WriteLine("2. Search for customer by name");
    Console.WriteLine("3. Add new customer");
    Console.WriteLine("4. Edit existing customer");
    Console.WriteLine("5. Delete existing customer");
    Console.WriteLine("6. View deleted customers log");
    Console.WriteLine("7. Back to main menu");

    var choice = Console.ReadLine()?.Trim() ?? string.Empty;

    switch (choice)
    {
        case "1":
            await CustomerHelper.ListCustomersAsync();
            break;
        case "2":
            var search = await SearchHelper.SearchNameAsync();
            await CustomerHelper.SearchCustomerAsync(search);
            break;
        case "3":
            await CustomerHelper.AddCustomerAsync();
            break;
        case "4":
            await CustomerHelper.ListCustomersAsync();
            var customerId = await IdHelper.GetCustomerId();
            await CustomerHelper.EditCustomerAsync(customerId);
            break;
        case "5":
            await CustomerHelper.ListCustomersAsync();
            var custId = await IdHelper.GetCustomerId();
            await CustomerHelper.DeleteCustomerAsync(custId);
            break;
        case "6":
            await CustomerHelper.ListDeletedCustomersAsync();
            break;
        case "7":
            Console.WriteLine("Back to main menu...");
            break;
        default:
            Console.WriteLine("Invalid input: Enter a number between 1 and 5!");
            break;
    }
}

// ------------------------------ Order Menu
static async Task OrderMenuAsync()
{
    Console.WriteLine("\n ---- Order Menu ----");
    Console.WriteLine("1. List orders");
    Console.WriteLine("2. Show order details for a specific order");
    Console.WriteLine("3. Add new order");
    Console.WriteLine("4. Edit existing order");
    Console.WriteLine("5. Delete existing order");
    Console.WriteLine("6. Back to main menu");

    var choice = Console.ReadLine()?.Trim() ?? string.Empty;

    switch (choice)
    {
        case "1":
            await OrderListMenuAsync();
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
            await OrderHelper.UpdateOrderAsync();
            break;
        case "5":
            await OrderHelper.ListOrdersAsync();
            var ordId = await IdHelper.GetOrderId();
            await OrderHelper.DeleteOrderAsync(ordId);
            break;
        case "6":
            Console.WriteLine("Back to main menu...");
            break;
        default:
            Console.WriteLine("Invalid input: Enter a number between 1 and 5!");
            break;
    }
}

// ------------------------------ OrderList Menu
static async Task OrderListMenuAsync()
{
    Console.WriteLine("\n ---- List Orders Menu ----");
    Console.WriteLine("1. List all orders");
    Console.WriteLine("2. Filter orders by status");
    Console.WriteLine("3. Filter orders by customer");
    Console.WriteLine("4. Paginated list of orders");
    Console.WriteLine("5. Back to previous menu");

    var choice = Console.ReadLine()?.Trim() ?? string.Empty;

    switch (choice)
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
            var pageInput = Console.ReadLine() ?? "1";
            var page = int.TryParse(pageInput, out var p) && p > 0 ? p : 1;
            Console.Write("Enter page size: ");
            var pageSizeInput = Console.ReadLine() ?? "10";
            var pageSize = int.TryParse(pageSizeInput, out var ps) && ps > 0 ? ps : 10;
            await OrderHelper.ListOrdersPagedAsync(page, pageSize);
            break;
        case "5":
            Console.WriteLine("Back to previous menu...");
            break;
        default:
            Console.WriteLine("Invalid input: Enter a number between 1 and 5!");
            break;
    }
}

// ------------------------------ Product Menu
static async Task ProductMenuAsync()
{
    Console.WriteLine("\n ---- Product Menu ----");
    Console.WriteLine("1. List all products");
    Console.WriteLine("2. Add new product");
    Console.WriteLine("3. Edit existing product");
    Console.WriteLine("4. Delete existing product");
    Console.WriteLine("5. View product sales");
    Console.WriteLine("6. Back to main menu");

    var choice = Console.ReadLine()?.Trim() ?? string.Empty;

    switch (choice)
    {
        case "1":
            await ProductHelper.ListProductsAsync();
            break;
        case "2":
            await ProductHelper.AddProductAsync();
            break;
        case "3":
            await ProductHelper.ListProductsAsync();
            var productId = await IdHelper.GetProductId();
            await ProductHelper.EditProductAsync(productId);
            break;
        case "4":
            await ProductHelper.ListProductsAsync();
            var prodId = await IdHelper.GetProductId();
            await ProductHelper.DeleteProductAsync(prodId);
            break;
        case "5":
            await ProductHelper.ListProductSalesAsync();
            break;
        case "6":
            Console.WriteLine("Back to main menu...");
            break;
        default:
            Console.WriteLine("Invalid input: Enter a number between 1 and 5!");
            break;
    }
}

// ------------------------------ Category Menu
static async Task CategoryMenuAsync()
{
    Console.WriteLine("\n ---- Category Menu ----");
    Console.WriteLine("1. List all categories");
    Console.WriteLine("2. Add new category");
    Console.WriteLine("3. Edit existing category");
    Console.WriteLine("4. Delete existing category");
    Console.WriteLine("5. View category sales");
    Console.WriteLine("6. Back to main menu");

    var choice = Console.ReadLine()?.Trim() ?? string.Empty;

    switch (choice)
    {
        case "1":
            await CategoryHelper.ListCategoriesAsync();
            break;
        case "2":
            await CategoryHelper.AddCategoryAsync();
            break;
        case "3":
            await CategoryHelper.ListCategoriesAsync();
            var catId = await IdHelper.GetCategoryId();
            await CategoryHelper.EditCategoryAsync(catId);
            break;
        case "4":
            await CategoryHelper.ListCategoriesAsync();
            var cId = await IdHelper.GetCategoryId();
            await CategoryHelper.DeleteCategoryAsync(cId);
            break;
        case "5":
            await CategoryHelper.ListCategorySalesAsync();
            break;
        case "6":
            Console.WriteLine("Back to main menu...");
            break;
        default:
            Console.WriteLine("Invalid input: Enter a number between 1 and 5!");
            break;
    }
}
