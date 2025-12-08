namespace Database_Project.Models;

public class DeletedCustomerLog
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public string? CustomerCity { get; set; }
    public DateTime DeletedAt { get; set; } = DateTime.Now;
}