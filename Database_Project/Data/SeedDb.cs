namespace Database_Project.Data;

public class SeedDb
{
    public static async Task SeedAsync()
    {
        await using var db = new ShopContext();

        // Ensures DB is created and applies migrations
        await db.Database.MigrateAsync();

        // Seed ProductCategories
        if (!await db.ProductCategories.AnyAsync())
        {
            db.ProductCategories.AddRange(
                new ProductCategory
                {
                    ProductCategoryName = "CD",
                    ProductCategoryDescription = "Compact Disc with inlet"
                },
                new ProductCategory
                {
                    ProductCategoryName = "Vinyl",
                    ProductCategoryDescription = "LPs and EPs, digital copy included."
                },
                new ProductCategory
                {
                    ProductCategoryName = "Cassette",
                    ProductCategoryDescription = "Quality may degrade over time."
                },
                new ProductCategory
                {
                    ProductCategoryName = "Digital",
                    ProductCategoryDescription = "Digital copy in downloadable format MP3 or WAV."
                }
            );
            await db.SaveChangesAsync();
            Console.WriteLine("Seeded product categories!");
        }

        // Fetching categories for FK assignment in Products
        var categories = await db.ProductCategories.AsNoTracking().ToListAsync();

        // Seeding Products
        if (!await db.Products.AnyAsync())
        {
            db.Products.AddRange(
                new Product
                {
                    ProductName = "Dina Ögon – Oas",
                    PricePerUnit = 199m,
                    ProductsInStock = 500,
                    ProductCategoryId = categories.First(c => c.ProductCategoryName == "CD").ProductCategoryId
                },
                new Product
                {
                    ProductName = "Shuggie Otis – Freedom Flight",
                    PricePerUnit = 299m,
                    ProductsInStock = 250,
                    ProductCategoryId = categories.First(c => c.ProductCategoryName == "Vinyl").ProductCategoryId
                },
                new Product
                {
                    ProductName = "Nationalteatern – Barn av vår tid",
                    PricePerUnit = 150m,
                    ProductsInStock = 100,
                    ProductCategoryId = categories.First(c => c.ProductCategoryName == "Cassette").ProductCategoryId
                },
                new Product
                {
                    ProductName = "Braxton Cook – No Doubt",
                    PricePerUnit = 89m,
                    ProductsInStock = 5000,
                    ProductCategoryId = categories.First(c => c.ProductCategoryName == "Digital").ProductCategoryId
                }
            );
            await db.SaveChangesAsync();
            Console.WriteLine("Seeded products!");
        }
        
        // Seeding Customers
        if (!await db.Customers.AnyAsync())
        {
            db.Customers.AddRange(
                new Customer
                {
                    CustomerName = "Alex Baldwin",
                    CustomerEmail = EncryptionHelper.Encrypt("bald@hotmail.com"),
                    CustomerCity = "Stockholm"
                },
                new Customer
                {
                    CustomerName = "Fred Wesley",
                    CustomerEmail = EncryptionHelper.Encrypt("horn@gmail.com"),
                    CustomerCity = "New York"
                },
                new Customer
                {
                    CustomerName = "Mock Customer",
                    CustomerEmail = EncryptionHelper.Encrypt("mock@mock.com"),
                    CustomerCity = "Göteborg"
                },
                new Customer
                {
                    CustomerName = "Maceo Parker",
                    CustomerEmail = EncryptionHelper.Encrypt("jbs@gmail.com"),
                    CustomerCity = "Norrköping"
                }
            );
            await db.SaveChangesAsync();
            Console.WriteLine("Seeded customers!");
        }

        // Fetching customers and products for FK assignment in Orders
        var customers = await db.Customers.AsNoTracking().ToListAsync();
        var products = await db.Products.AsNoTracking().ToListAsync();
        
        // Seeding Orders with OrderRows
        if (!await db.Orders.AnyAsync())
        {
            db.Orders.AddRange(
                new Order
                {
                    CustomerId = customers.First(c => 
                        c.CustomerEmail == EncryptionHelper.Encrypt("bald@hotmail.com")).CustomerId,
                    OrderDate = DateTime.Today.AddDays(-1),
                    OrderStatus = "Pending",
                    OrderRows = new List<OrderRow>
                    {
                        new OrderRow
                        {
                            ProductId = products.First(p => p.ProductName == "Dina Ögon – Oas").ProductId,
                            OrderRowQuantity = 1,
                            OrderRowTotal = products.First(p => p.ProductName == "Dina Ögon – Oas").PricePerUnit * 1
                        }
                    },
                    OrderTotal = products.First(p => p.ProductName == "Dina Ögon – Oas").PricePerUnit * 1
                },
                new Order
                {
                    CustomerId = customers.First(c => 
                        c.CustomerEmail == EncryptionHelper.Encrypt("horn@gmail.com")).CustomerId,
                    OrderDate = DateTime.Today.AddDays(-2),
                    OrderStatus = "Paid",
                    OrderRows = new List<OrderRow>
                    {
                        new OrderRow
                        {
                            ProductId = products.First(p => p.ProductName == "Shuggie Otis – Freedom Flight").ProductId,
                            OrderRowQuantity = 2,
                            OrderRowTotal = products.First(p => p.ProductName == "Shuggie Otis – Freedom Flight").PricePerUnit * 2
                        }
                    },
                    OrderTotal = products.First(p => p.ProductName == "Shuggie Otis – Freedom Flight").PricePerUnit * 2
                },
                new Order
                {
                    CustomerId = customers.First(c => 
                        c.CustomerEmail == EncryptionHelper.Encrypt("mock@mock.com")).CustomerId,
                    OrderDate = DateTime.Today.AddDays(-3),
                    OrderStatus = "Shipped",
                    OrderRows = new List<OrderRow>
                    {
                        new OrderRow
                        {
                            ProductId = products.First(p => p.ProductName == "Nationalteatern – Barn av vår tid").ProductId,
                            OrderRowQuantity = 3,
                            OrderRowTotal = products.First(p => p.ProductName == "Nationalteatern – Barn av vår tid").PricePerUnit * 3
                        }
                    },
                    OrderTotal = products.First(p => p.ProductName == "Nationalteatern – Barn av vår tid").PricePerUnit * 3
                },
                new Order
                {
                    CustomerId = customers.First(c => 
                        c.CustomerEmail == EncryptionHelper.Encrypt("jbs@gmail.com")).CustomerId,
                    OrderDate = DateTime.Today.AddDays(-4),
                    OrderStatus = "Shipped",
                    OrderRows = new List<OrderRow>
                    {
                        new OrderRow
                        {
                            ProductId = products.First(p => p.ProductName == "Braxton Cook – No Doubt").ProductId,
                            OrderRowQuantity = 1,
                            OrderRowTotal = products.First(p => p.ProductName == "Braxton Cook – No Doubt").PricePerUnit * 1
                        }
                    },
                    OrderTotal = products.First(p => p.ProductName == "Braxton Cook – No Doubt").PricePerUnit * 1
                }
            );

            await db.SaveChangesAsync();
            Console.WriteLine("Seeded orders with order rows!");
        }
    }
}
