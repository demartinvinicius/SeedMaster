namespace POC.Model;

public class Product
{
    public Guid Id { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public List<OrderItems> OrderItems { get; set; }
    public Supplier Supplier { get; set; }
}