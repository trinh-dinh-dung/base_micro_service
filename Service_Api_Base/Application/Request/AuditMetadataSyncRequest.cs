using System.Collections.Generic;

namespace Application.Request
{
    public class AuditMetadataSyncRequest
    {
        public string PageCode { get; set; }

        public List<AuditMetadataSyncItem> Items { get; set; } = new();
    }

    public class AuditMetadataSyncItem
    {
        public string FieldName { get; set; }

        public string LabelVi { get; set; }

        public string LabelEn { get; set; }

        public string TranslationKey { get; set; }
    }
}