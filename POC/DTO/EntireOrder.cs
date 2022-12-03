namespace POC.DTO;

public class EntireOrder
{
    public string ConsumerName { get; set; }
    public DateTime OrderTime { get; set; }
    public decimal TotalPrice { get; set; }
    public List<EachOrderItem> OrderItems { get; set; }
}

public class EachOrderItem
{
    public string SupplierName { get; set; }
    public string ProductName { get; set; }
    public uint QuantityOrdered { get; set; }
    public decimal UnitPrice { get; set; }
}