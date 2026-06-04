using Domain.Entities;
using Infrastructure.DataContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Base.Audit
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuditInterceptor> _logger;

        public AuditInterceptor(IHttpContextAccessor httpContextAccessor, ILogger<AuditInterceptor> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            AppendAuditLogs(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            AppendAuditLogs(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void AppendAuditLogs(DbContext context)
        {
            if (context is not PVIContext db)
            {
                return;
            }

            var httpContext = _httpContextAccessor.HttpContext;
            var pageCode = httpContext?.Request?.Headers["page_code"].FirstOrDefault()
                           ?? httpContext?.Request?.Query["page_code"].FirstOrDefault();
            var traceId = httpContext?.TraceIdentifier;
            var requestPath = httpContext?.Request?.Path.Value;
            var requestMethod = httpContext?.Request?.Method;
            var userName = httpContext?.User?.Identity?.Name;
            var statusCode = httpContext?.Response?.StatusCode;

            var changedCount = 0;

            foreach (var entry in db.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified)
                .Where(e => e.Metadata.ClrType != typeof(EntityAuditLogs)))
            {
                var entityName = entry.Metadata.GetTableName() ?? entry.Metadata.ClrType.Name;
                var keyValue = ResolveEntityId(entry);

                foreach (var property in entry.Properties.Where(p => p.IsModified && !p.Metadata.IsPrimaryKey()))
                {
                    var oldValue = ToAuditValue(property.OriginalValue);
                    var newValue = ToAuditValue(property.CurrentValue);

                    if (string.Equals(oldValue, newValue, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    db.EntityAuditLogs.Add(new EntityAuditLogs
                    {
                        Id = Guid.NewGuid(),
                        EntityName = entityName,
                        Action = "Modified",
                        EntityKeys = $"{keyValue};FieldName={property.Metadata.Name}",
                        OldValues = oldValue,
                        NewValues = newValue,
                        PageCode = pageCode,
                        TraceId = traceId,
                        RequestPath = requestPath,
                        RequestMethod = requestMethod,
                        UserName = userName,
                        StatusCode = statusCode,
                        ChangedAtUtc = DateTimeOffset.UtcNow,
                    });

                    changedCount++;
                }
            }

            if (changedCount > 0)
            {
                _logger.LogInformation("AuditInterceptor captured {Count} changed field(s)", changedCount);
            }
        }

        private static string ResolveEntityId(EntityEntry entry)
        {
            var primaryKey = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey());
            if (primaryKey == null)
            {
                return null;
            }

            var idValue = primaryKey.CurrentValue ?? primaryKey.OriginalValue;
            return $"{primaryKey.Metadata.Name}={ToAuditValue(idValue)}";
        }

        private static string ToAuditValue(object value)
        {
            return value?.ToString() ?? "null";
        }
    }
}