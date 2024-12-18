namespace BudgetFlow.Application.Common.Interfaces.Services
{
    public interface IPasswordHasher
    {
        string Hash(string password);
    }
}
