namespace Database_Project.Helpers;

public static class CustomerHelper
{
    /// <summary>
    /// Lists customers.
    /// </summary>
    public static async Task ListCustomersAsync()
    {
        await using var db = new ShopContext();
    
        var customers = await db.Customers.AsNoTracking().OrderBy(c => c.CustomerId).ToListAsync();
    
        Console.WriteLine($"\n{"Customers", 25}");
        Console.WriteLine($"{"ID",-4} | {"Name",-25} | {"City",-25} | {"Email",-25}");
        Console.WriteLine(new string('-', 85));

        foreach (var customer in customers)
        {
            var decryptedEmail = EncryptionHelper.Decrypt(customer.CustomerEmail ?? string.Empty);
            
            Console.WriteLine($"{customer.CustomerId,-4} | {customer.CustomerName,-25} |" +
                              $" {customer.CustomerCity,-25} | {decryptedEmail,-25}");
        }
    }
    
    /// <summary>
    /// Adds a new customer.
    /// </summary>
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
            CustomerEmail = EncryptionHelper.Encrypt(customerEmail),
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

    /// <summary>
    /// Edits a customer.
    /// </summary>
    /// <param name="customerId">ID of customer to be edited.</param>
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
            customer.CustomerEmail = EncryptionHelper.Encrypt(email);
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

   /// <summary>
   /// Deletes a customer.
   /// </summary>
   /// <param name="custId">ID of customer to be deleted.</param>
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
    
    /// <summary>
    /// Searches for customers by name.
    /// </summary>
    /// <param name="search">Query to compare against customer names.</param>
    public static async Task SearchCustomerAsync(string search)
    {
        await using var  db = new ShopContext();
    
        if (string.IsNullOrWhiteSpace(search))
        {
            Console.WriteLine("Search cannot be empty.");
            return;
        }
    
        var customers = await db.Customers
            .AsNoTracking()
            .OrderBy(b => b.CustomerName)
            .ToListAsync();
    
        var filteredCustomers = customers
            .Where(b => b.CustomerName != null && b.CustomerName.Contains(search, StringComparison.OrdinalIgnoreCase))
            .ToList();
   
        if (filteredCustomers.Count == 0)
        {
            Console.WriteLine($"No customers found matching '{search}'.");
            return;
        }
    
        Console.WriteLine($"\n{"Customers", 25}");
        Console.WriteLine($"{"ID",-4} | {"Name",-25} | {"City",-25} | {"Email",-25}");
        Console.WriteLine(new string('-', 85));

        foreach (var customer in filteredCustomers)
        {
            var decryptedEmail = EncryptionHelper.Decrypt(customer.CustomerEmail ?? string.Empty);
            
            Console.WriteLine($"{customer.CustomerId,-4} | {customer.CustomerName,-25} |" +
                              $" {customer.CustomerCity,-25} | {decryptedEmail,-25}");
        }
    }
    
    /// <summary>
    /// Shows deleted customers log (log created by trigger).
    /// </summary>
    public static async Task ListDeletedCustomersAsync()
    {
        await using var db = new ShopContext();

        var logs = await db.Set<DeletedCustomerLog>()
            .AsNoTracking()
            .OrderByDescending(log => log.DeletedAt)
            .ToListAsync();

        Console.WriteLine($"\n{"Deleted Customers Log",50}");
        Console.WriteLine($"{"ID",-4} | {"Name",-25} | {"Email",-30} | {"City",-20} | {"Deleted At",-20}");
        Console.WriteLine(new string('-', 110));

        foreach (var log in logs)
        {
            var decryptedEmail = EncryptionHelper.Decrypt(log.CustomerEmail ?? string.Empty);
            
            Console.WriteLine($"{log.CustomerId,-4} | {log.CustomerName,-25} | {decryptedEmail,-30} | " +
                              $"{log.CustomerCity,-20} | {log.DeletedAt,-20:yyyy-MM-dd HH:mm}");
        }
    }
}
