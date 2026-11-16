namespace BalticAmadeusTask.Models;

public class Result
{
    public string Id { get; set; } = "";
    public decimal NetFuelPrice { get; set; }
    public decimal LogisticsCost { get; set; }
    public decimal Excise { get; set; }
    public decimal VAT { get; set; }
    public decimal TotalTax { get; set; }
    public decimal GrandTotal { get; set; }
}