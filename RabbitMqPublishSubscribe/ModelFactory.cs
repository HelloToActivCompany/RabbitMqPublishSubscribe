using System;
using RabbitMQ.Client;


namespace RabbitMqPublishSubscribe
{
	public class ModelFactory : IModelFactory
	{
		private readonly IConnection _connection;

		public ModelFactory(string ampqConnectionString)
		{
			ConnectionFactory connectionFactory = new ConnectionFactory {Uri = ampqConnectionString};
			_connection = connectionFactory.CreateConnection();
		}

		public IModel CreateModel()
		{
			return _connection.CreateModel();
		}
	}
}
