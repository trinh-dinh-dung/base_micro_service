using System;

namespace Application.GetMap
{
    public class DepartmentRow
    {
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }
        public Guid? ParentId { get; set; }
        public string Note { get; set; }
        public int? Level { get; set; }

        public DepartmentTree ToDepartmentTree() => new()
        {
            DepartmentId = DepartmentId,
            DepartmentName = DepartmentName,
            DepartmentCode = DepartmentCode,
            ParentId = ParentId ?? Guid.Empty,
            Note = Note,
            Level = Level
        };
    }
}
