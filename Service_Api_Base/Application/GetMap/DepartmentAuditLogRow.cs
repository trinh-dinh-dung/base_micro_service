using System;

namespace Application.GetMap
{
    public class DepartmentAuditLogRow
    {
        public Guid Id { get; set; }
        public string Action { get; set; }
        public string EntityKeys { get; set; }
        public string FieldName { get; set; }
        public string FieldLabel { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string PageCode { get; set; }
        public string RequestMethod { get; set; }
        public string RequestPath { get; set; }
        public string UserName { get; set; }
        public string TraceId { get; set; }
        public int? StatusCode { get; set; }
        public DateTimeOffset ChangedAtUtc { get; set; }
    }
}