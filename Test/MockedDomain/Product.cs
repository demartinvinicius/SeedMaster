namespace Test.MockedDomain;


public class Product
{
    public Guid Id { get; set; }
    public string ProductName { get; set; }
    public double Price { get; set; }
    public List<OrderItems> OrderItems { get; set; }
    public Supplier Supplier { get; set; }
}
