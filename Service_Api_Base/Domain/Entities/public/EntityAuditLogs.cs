using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public partial class EntityAuditLogs
    {
        public EntityAuditLogs()
        {
            #region Generated Constructor
            #endregion
        }

        #region Generated Properties
        public Guid Id { get; set; }

        public string EntityName { get; set; } = null!;

        public string Action { get; set; } = null!;

        public string? EntityKeys { get; set; }

        public string? OldValues { get; set; }

        public string? NewValues { get; set; }

        public string? PageCode { get; set; }

        public string? TraceId { get; set; }

        public string? RequestPath { get; set; }

        public string? RequestMethod { get; set; }

        public string? UserName { get; set; }

        public int? StatusCode { get; set; }

        public DateTimeOffset ChangedAtUtc { get; set; }

        #endregion

        #region Generated Relationships
        #endregion

    }
}
