using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database_Project.Migrations
{
    /// <inheritdoc />
    public partial class ViewsAndTriggerUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
             // DeletedCustomersLog table
            migrationBuilder.CreateTable(
                name: "DeletedCustomersLog",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: false),
                    CustomerEmail = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerCity = table.Column<string>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table => { }
            );
            
            migrationBuilder.Sql(@"
            CREATE TRIGGER IF NOT EXISTS trg_LogDeletedCustomer
            AFTER DELETE ON Customers
            BEGIN
                INSERT INTO DeletedCustomersLog (CustomerId, CustomerName, CustomerEmail, CustomerCity, DeletedAt)
                VALUES (OLD.CustomerId, OLD.CustomerName, OLD.CustomerEmail, OLD.CustomerCity, CURRENT_TIMESTAMP);
            END;
            ");
            
            migrationBuilder.Sql(@"
            CREATE VIEW IF NOT EXISTS ProductSales AS
            SELECT
                p.ProductId,
                p.ProductName,
                p.PricePerUnit,
                pc.ProductCategoryId,
                pc.ProductCategoryName,
                SUM(orw.OrderRowQuantity) AS TotalQuantity,
                SUM(orw.OrderRowTotal) AS TotalSales
            FROM OrderRows orw
            JOIN Products p ON orw.ProductId = p.ProductId
            JOIN ProductCategories pc ON p.ProductCategoryId = pc.ProductCategoryId
            GROUP BY p.ProductId, p.ProductName, p.PricePerUnit, pc.ProductCategoryId, pc.ProductCategoryName;
            ");
            
            migrationBuilder.Sql(@"
            CREATE VIEW IF NOT EXISTS CategorySales AS
            SELECT
                pc.ProductCategoryId,
                pc.ProductCategoryName,
                SUM(orw.OrderRowQuantity) AS TotalQuantity,
                SUM(orw.OrderRowTotal) AS TotalSales
            FROM OrderRows orw
            JOIN Products p ON orw.ProductId = p.ProductId
            JOIN ProductCategories pc ON p.ProductCategoryId = pc.ProductCategoryId
            GROUP BY pc.ProductCategoryId, pc.ProductCategoryName;
            ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeletedCustomersLog");
            
            migrationBuilder.Sql(@"
            DROP VIEW IF EXISTS CategorySales;
            ");
            
            migrationBuilder.Sql(@"
            DROP VIEW IF EXISTS ProductSales;
            ");
            
            migrationBuilder.Sql(@"
            DROP TRIGGER IF EXISTS trg_LogDeletedCustomer;
            ");
        }
    }
}
