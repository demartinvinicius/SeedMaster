namespace Test.MockedDomain;

public class Supplier
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string CNPJ { get; set; }
    public List<Product> Products { get; set; }
}