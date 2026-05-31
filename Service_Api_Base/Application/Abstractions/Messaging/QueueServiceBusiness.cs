namespace Application.Abstractions.Messaging
{
    public class QueueServiceBusiness
    {
        public string QueueName { get; set; }
        public string RequesetId { get; set; }
        public int BusinessServiceType { get; set; }
        public int BusinessType { get; set; }
        public int Status { get; set; }
        public string TenantName { get; set; }
        public int? CategoryType { get; set; }
        public string BusinessDocName { get; set; }
        public string DocID { get; set; }
        public object Data { get; set; }
    }
}
