using BudgetFlow.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetFlow.Application.Common.Utils
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(e => e.ID);
            builder.Property(e => e.Token).HasMaxLength(200);
            builder.HasIndex(e => e.Token).IsUnique();
            builder.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserID);
        }
    }
}
