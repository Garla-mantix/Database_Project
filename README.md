# EFC Shop – SQLite database with Entity Framework Core

## Overview
EFC Shop is a .NET 8 console application that manages customers, products, orders, and categories using Entity Framework Core and SQLite.
It demonstrates relational modeling, CRUD operations, transactions, database triggers and views, as well as encryption.
The shop mimicks a webshop for music media like CD's, vinyl, cassettes etc.

### ER Model
The system follows a 3rd Normal Form (3NF) relational structure and consists of the following entities and relationships:

*   **Customer** `(1:0–N)` **Order**: A customer can have zero to multiple orders.
*   **Order** `(1:1–N)` **OrderRow**: An order consists of one or more rows (items).
*   **OrderRow** `(N:1)` **Product**: Each order row refers to one specific product.
*   **Product** `(N:1)` **ProductCategory**: A product belongs to one category.
*   **Admin**: A standalone table for administrative access.
<br>

<img width="1100" height="808" alt="ER-diagram" src="https://github.com/user-attachments/assets/0d12a87b-cd7b-4b0b-a02d-85255a813984" />

### Core CRUD Functionality
All operations use EF Core with LINQ for querying and filtering.
*   **Create**: Create customers, orders, products and categories, with validation checks for empty names and duplicates.
*   **Read**: Listings with joined data (e.g., Product with Category name). Search functionality for customer names.
*   **Update**: Allows partial updates (updating price without changing name), and correctly adjusts stock count for products when orders change.
*   **Delete**: Protected by validation (cannot delete a category containing products). Trigger logs customer deletions.
<br>

## Security

### Password Hashing
One admin user is seeded. As of now there is no CRUD functionality for Admins, only the seeded admin exists.
However the user password for the admin is stored in the database hashed and salted, and user input is masked with asterisks when typing. 
When only using hashing, common passwords gets the same hash, creating a security flaw. By adding salt this problem is circumvented and each password gets a unique hash, even between identical passwords.
*   **Mechanism**: SHA-512 (HMACSHA512).
*   **Salting**: A unique random salt is generated.
*   **Verification**: During login, the entered password is hashed with the stored salt and compared to the stored hash.

### Encryption
Application-level encryption is applied to sensitive customer data.
*   **Field**: `Customer.CustomerEmail`.
*   **Implementation**: The `EncryptionHelper` class performs XOR encryption before saving to the database and decryption when reading back to the UI.
<br>

## Transactions

### Order Processing
*   **Stock Management**: When an order is placed, the system checks `ProductsInStock`. If sufficient, the stock is decremented.
*   **Transactions**: The entire order placement process (creating the order, adding rows, updating stock) is wrapped in an **EF Core Transaction**. If any step fails (e.g., insufficient stock for the second item), `RollbackAsync()` is called, reverting all changes.

### Optimization & Pagination
*   **Pagination**: The `ListOrdersPagedAsync` method implements pagination using `.Skip()` and `.Take()`.
*   **Sorting and filtering**: Orders are sorted and filtered by multiple columns implicitly or explicitly in different views (e.g. orders per customer).
*   **Optimized Reading**: `AsNoTracking()` is used for read-only lists (like product catalogs) to improve performance.
<br>

## Database Objects

### Trigger
The SQL trigger `trg_LogDeletedCustomer` is implemented to create a log for deletions.
*   **Function**: When a `Customer` is deleted, their details are automatically inserted into a `DeletedCustomersLog` table.
*   **Usage**: Accessible via the "View deleted customers log" menu option.

### Views
Two SQL views were created to simplify sales reporting:
1.  `ProductSales`: Aggregates total quantity sold and revenue made per product.
2.  `CategorySales`: Aggregates total quantity sold and revenue made per category.
*   **Usage**: These are mapped to Keyless Entity Types (`ProductSalesView`, `CategorySalesView`) in EF Core, allowing efficient querying for sales reports.

## Known Limitations
* Only one seeded admin account and no CRUD functionality for new or existing admins.
* Only basic XOR encryption using Base64 for emails.
* Order status is a static propery which never changes.
* Limited usability of the paginated listing of orders since we cannot jump between pages.
* Currently only filtered listing for orders, not for products etc. (E.g. filtering products by category could be useful).
* Search is only implemented for customer names.
* No extensive indexing for increased performance yet.

## How to use
1. Install the .NET 8 SDK.
2. Clone the repository from GitHub (https://github.com/Garla-mantix/Database_Project.git).
3. Navigate to the correct folder in the terminal, and type the commands below.
4. Run migrations: "dotnet ef database update".
5. Start the program: "dotnet run".
   
**Login credentials**:<br>
Username: "admin" <br>
Password: "securepassword"

## Author
**Luca Pirro**  
_Built as a learning project._


