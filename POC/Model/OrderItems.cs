namespace POC.Model;

public class OrderItems
{
    public Guid Id { get; set; }
    public Order Order { get; set; }
    public Guid ProductId { get; set; }
    public Guid OrderId { get; set; }
    public Product Product { get; set; }
    public uint Qty { get; set; }
}