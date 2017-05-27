using RabbitMQ.Client;

namespace RabbitMqPublishSubscribe
{
    public interface IModelFactory
    {
		IModel CreateModel();
    }
}
