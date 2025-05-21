namespace BudgetFlow.Application.Common.Dtos;
public class EntryDto
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public decimal AmountInTRY { get; set; }
    public DateTime Date { get; set; }
    public int CategoryID { get; set; }
    public int WalletID { get; set; }
}
