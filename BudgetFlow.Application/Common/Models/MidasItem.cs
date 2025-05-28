namespace BudgetFlow.Application.Common.Models;
public class MidasItem
{
    public string _id { get; set; }
    public string Code { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Last { get; set; }
    public decimal Close { get; set; }
    public decimal PreviousClose { get; set; }
    public long DateTime { get; set; }
    public decimal DailyChange { get; set; }
    public decimal DailyChangePercent { get; set; }
    public decimal Ask { get; set; }  // Satış
    public decimal Bid { get; set; }  // Alış
}
