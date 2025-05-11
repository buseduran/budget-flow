namespace BudgetFlow.Domain.Entities;
public class Role
{
    public const string Admin = "Admin";
    public const string Member = "Member";
    public const int AdminID = 1;
    public const int MemberID = 2;

    public int ID { get; init; }
    public string Name { get; init; }

}

