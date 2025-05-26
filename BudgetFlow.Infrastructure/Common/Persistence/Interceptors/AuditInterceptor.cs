using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Common;
using BudgetFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;

namespace BudgetFlow.Infrastructure.Common.Persistence.Interceptors;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;
    private readonly List<(EntityEntry Entry, AuditLog Log, EntityState OriginalState)> _pendingLogs = new();

    public AuditInterceptor(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null) return base.SavingChangesAsync(eventData, result, cancellationToken);

        var userID = _currentUserService.GetCurrentUserID();

        _pendingLogs.Clear();

        var entries = context.ChangeTracker.Entries()
            .Where(e =>
                e.Entity is IAuditableEntity &&
                e.Entity is not AuditLog &&
                (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted));

        foreach (var entry in entries)
        {
            var auditLog = new AuditLog
            {
                TableName = entry.Entity.GetType().Name,
                Action = entry.State.ToString(),
                UserID = userID,
                Timestamp = DateTime.UtcNow,
                PrimaryKey = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue?.ToString() ?? "",
                OldValues = entry.State != EntityState.Added
                    ? SerializeProperties(entry.OriginalValues.Properties, entry.OriginalValues)
                    : null,
                NewValues = entry.State != EntityState.Deleted
                    ? SerializeProperties(entry.CurrentValues.Properties, entry.CurrentValues)
                    : null
            };

            _pendingLogs.Add((entry, auditLog, entry.State));
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null || !_pendingLogs.Any()) return await base.SavedChangesAsync(eventData, result, cancellationToken);

        foreach (var (entry, log, originalState) in _pendingLogs)
        {
            var pk = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue;
            log.PrimaryKey = pk?.ToString() ?? "";

            //if (originalState != EntityState.Deleted)
            //{
            //    log.NewValues = SerializeProperties(entry.CurrentValues.Properties, entry.CurrentValues);
            //}

            //if (originalState != EntityState.Added)
            //{
            //    log.OldValues = SerializeProperties(entry.OriginalValues.Properties, entry.OriginalValues);
            //}
        }

        context.Set<AuditLog>().AddRange(_pendingLogs.Select(x => x.Log));
        await context.SaveChangesAsync(cancellationToken);

        _pendingLogs.Clear();

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private static string SerializeProperties(IEnumerable<Microsoft.EntityFrameworkCore.Metadata.IProperty> props, PropertyValues values)
    {
        var dict = props.ToDictionary(p => p.Name, p => values[p.Name]);
        return JsonSerializer.Serialize(dict);
    }
}
