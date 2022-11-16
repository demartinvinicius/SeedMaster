namespace POC.Model;

public class Order
{
    public Guid Id { get; set; }
    public DateTime OrderTime { get; set; }
    public Person Person { get; set; }
    public List<OrderItems> OrderItems { get; set; }
    public Guid PersonId { get; set; }
}