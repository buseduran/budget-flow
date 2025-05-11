using BudgetFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetFlow.Infrastructure.Common.Configuration;
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.ID);
        builder.Property(r => r.Name).IsRequired().HasMaxLength(50);

        builder.HasData(
            new Role { ID = Role.AdminID, Name = Role.Admin },
            new Role { ID = Role.MemberID, Name = Role.Member }
        );
    }
}
