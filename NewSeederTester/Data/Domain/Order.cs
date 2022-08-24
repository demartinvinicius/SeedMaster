namespace NewSeederTester.Data.Domain;

public class Order
{
    public Guid Id { get; set; }
    public DateTime OrderTime { get; set; }
    public Person Person { get; set; }
    public List<OrderItems> OrderItems { get; set; }
}