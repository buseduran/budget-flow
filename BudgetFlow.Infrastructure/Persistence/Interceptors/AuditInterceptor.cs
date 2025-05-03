using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BudgetFlow.Infrastructure.Persistence.Interceptors;

public class AuditInterceptor:SaveChangesInterceptor
{
   //public override async Task<int> SaveChangesAsync(
   //    SaveChangesEventData eventData,
   //    CancellationToken cancellationToken = default)
   // {
   //     var entries = eventData.Context.ChangeTracker.Entries()
   //         .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
   //     foreach (var entry in entries)
   //     {
   //         if (entry.Entity is IAuditableEntity auditableEntity)
   //         {
   //             if (entry.State == EntityState.Added)
   //             {
   //                 auditableEntity.CreatedAt = DateTime.UtcNow;
   //             }
   //             else
   //             {
   //                 entry.Property(nameof(IAuditableEntity.CreatedAt)).IsModified = false;
   //             }
   //             auditableEntity.UpdatedAt = DateTime.UtcNow;
   //         }
   //     }
   //     return await base.SavedChangesAsync(eventData, cancellationToken);
   // }
}

