namespace Test.MockedDomain;

public class OrderItems
{
    public Guid Id { get; set; }
    public Order Order { get; set; }
    public Product Product { get; set; }
    public uint Qty { get; set; }
}