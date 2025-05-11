namespace BudgetFlow.Domain.Entities;
public class UserRole
{
    public int UserID { get; set; }
    public int RoleID { get; set; }
    public User User { get; set; }
    public Role Role { get; set; }

}
