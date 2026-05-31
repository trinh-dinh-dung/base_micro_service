namespace Application.Abstractions.Messaging
{
    public interface IRabbitMQClient
    {
        void SendRabbitMQClientQueues(QueueServiceBusiness queuesRequest);
    }
}
