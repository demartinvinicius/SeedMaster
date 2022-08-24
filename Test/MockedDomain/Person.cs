namespace Test.MockedDomain;


public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string CPF { get; set; }
    public List<Order> Orders { get; set; }
}
