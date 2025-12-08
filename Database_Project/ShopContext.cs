namespace Database_Project;

public class ShopContext : DbContext
{
    // Mapping to tables in database
    public DbSet<Customer> Customers =>  Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderRow> OrderRows => Set<OrderRow>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    
    // Table for logging deleted customers
    public DbSet<DeletedCustomerLog> DeletedCustomersLog { get; set; }
    
    // VIEWS
    public DbSet<CategorySalesView> CategorySales { get; set; }
    public DbSet<ProductSalesView> ProductSales { get; set; }
    
    // Telling EF Core to use SQLite and where to put the file
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = Path.Combine(AppContext.BaseDirectory, "ShopContext.db");
        optionsBuilder.UseSqlite($"Filename={dbPath}");
    }
    
    // OnModelCreating (fine-tuning the model)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Customer
        modelBuilder.Entity<Customer>(c =>
        {
            // PK
            c.HasKey(e => e.CustomerId);

            // Properties
            c.Property(e => e.CustomerName)
                .IsRequired()
                .HasMaxLength(100);
            
            c.Property(e => e.CustomerEmail)
                .IsRequired()
                .HasMaxLength(100);

            c.Property(e => e.CustomerCity)
                .HasMaxLength(100);

            // Creating UNIQUE-index for CustomerEmail
            c.HasIndex(e => e.CustomerEmail).IsUnique();
            
            // FK – One Customer can have many Orders
            c.HasMany(e => e.Orders)
                .WithOne(e => e.Customer)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Order
        modelBuilder.Entity<Order>(o =>
        {
            // PK
            o.HasKey(e => e.OrderId);

            // Properties
            o.Property(e => e.OrderDate)
                .IsRequired();
            
            o.Property(e => e.OrderStatus)
                .HasMaxLength(100);

            o.Property(e => e.OrderTotal)
                .IsRequired();
            
            // FK – One Order can have many OrderRows
            o.HasMany(e => e.OrderRows)
                .WithOne(e => e.Order)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // OrderRow
        modelBuilder.Entity<OrderRow>(or =>
        {
            // PK
            or.HasKey(e => e.OrderRowId);

            // Properties
            or.Property(e => e.OrderRowQuantity)
                .IsRequired();

            or.Property(e => e.OrderRowTotal)
                .IsRequired();

            // FK – One Product can have many OrderRows
            or.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // Product
        modelBuilder.Entity<Product>(p =>
        {
            // PK
            p.HasKey(e => e.ProductId);

            // Properties
            p.Property(e => e.ProductName)
                .IsRequired()
                .HasMaxLength(100);

            p.Property(e => e.PricePerUnit)
                .IsRequired();

            p.Property(e => e.ProductsInStock)
                .IsRequired();
            
            // UNIQUE-index for ProductName
            p.HasIndex(e => e.ProductName).IsUnique();
            
            // FK
            p.HasOne(e => e.ProductCategory)
                .WithMany(pc => pc.Products)
                .HasForeignKey(e => e.ProductCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // ProductCategory
        modelBuilder.Entity<ProductCategory>(pc =>
        {
            pc.HasKey(e => e.ProductCategoryId);
            pc.Property(e => e.ProductCategoryName)
                .IsRequired()
                .HasMaxLength(100);
            pc.Property(e => e.ProductCategoryDescription)
                .HasMaxLength(100);
            
            // UNIQUE-index for ProductCategoryName
            pc.HasIndex(e => e.ProductCategoryName).IsUnique();
        });
        
        // VIEWS
        // Category sales view
        modelBuilder.Entity<CategorySalesView>(e =>
        {
            e.HasNoKey();
            e.ToView("CategorySales");
        });

        // Product sales view
        modelBuilder.Entity<ProductSalesView>(e =>
        {
            e.HasNoKey();
            e.ToView("ProductSales");
        });
        
        // TRIGGER
        modelBuilder.Entity<DeletedCustomerLog>(e =>
        {
            e.HasNoKey();
            e.ToTable("DeletedCustomersLog"); 
        });
    }
}