namespace Database_Project.Helpers;

public static class CustomerHelper
{
    // Listing customers
    public static async Task ListCustomersAsync()
    {
        await using var db = new ShopContext();
    
        var customers = await db.Customers.AsNoTracking().OrderBy(c => c.CustomerId).ToListAsync();
    
        Console.WriteLine($"\n{"Customers", 25}");
        Console.WriteLine($"{"ID",-4} | {"Name",-25} | {"City",-25} | {"Email",-25}");
        Console.WriteLine(new string('-', 85));

        foreach (var customer in customers)
        {
            Console.WriteLine($"{customer.CustomerId,-4} | {customer.CustomerName,-25} |" +
                              $" {customer.CustomerCity,-25} | {customer.CustomerEmail,-25}");
        }
    }
    
    // Adding customers
    public static async Task AddCustomerAsync()
    {
        await using var db = new ShopContext();
        
        Console.Write("Name: ");
        var customerName = (Console.ReadLine() ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(customerName) || customerName.Length > 100)
        {
            Console.WriteLine("Name cannot be empty or more than 100 characters.");
            return;
        }
        
        Console.Write("City: ");
        var customerCity = (Console.ReadLine() ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(customerCity) || customerCity.Length > 100)
        {
            Console.WriteLine("City cannot be empty or more than 100 characters.");
            return;
        }
        
        Console.Write("Email: ");
        var customerEmail = (Console.ReadLine() ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(customerEmail) || customerEmail.Length > 100)
        {
            Console.WriteLine("Email cannot be empty or more than 100 characters.");
            return;
        }
        
        db.Customers.Add(new Customer
        {
            CustomerName = customerName,
            CustomerCity = customerCity,
            CustomerEmail = customerEmail,
        });
        
        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Customer added!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("DB error: " + ex.Message);
        }
    }

    // Editing customers
    public static async Task EditCustomerAsync(int customerId)
    {
        await using var db = new ShopContext();
        
        var customer = await db.Customers.FindAsync(customerId);

        if (customer == null)
        {
            Console.WriteLine("Customer not found.");
            return;
        }
        
        Console.Write("New name (leave empty to keep): ");
        var name = Console.ReadLine()?.Trim();
        if (!string.IsNullOrWhiteSpace(name))
        {
            customer.CustomerName = name;
        }
        
        Console.Write("New city (leave empty to keep): ");
        var city = Console.ReadLine()?.Trim();
        if (!string.IsNullOrWhiteSpace(city))
        {
            customer.CustomerCity = city;
        }
        
        Console.Write("New email (leave empty to keep): ");
        var email = Console.ReadLine()?.Trim();
        if (!string.IsNullOrWhiteSpace(email))
        {
            customer.CustomerEmail = email;
        }
        
        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Customer updated!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("DB error: " + ex.Message);
        }
    }

    // Deleting customer
    public static async Task DeleteCustomerAsync(int custId)
    {
        await using var db = new ShopContext();
        
        var customer = await db.Customers.FindAsync(custId);

        if (customer == null)
        {
            Console.WriteLine("Customer not found.");
            return;
        }
        
        Console.Write($"Are you sure you want to delete '{customer.CustomerName}'? (y/n): ");
        var confirm = Console.ReadLine()?.Trim().ToLower();
        if (confirm != "y")
        {
            Console.WriteLine("Deletion cancelled.");
            return;
        }
        
        db.Customers.Remove(customer);
        await db.SaveChangesAsync();
        Console.WriteLine("Customer deleted!");
    }
}
