using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using PasswordGeneratorInternalTypes.PublishSubscribe;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqPublishSubscribe
{
	public class RabbitMqManager : IPublishSubscribe, IDisposable
	{
		private readonly IModelFactory _modelFactory;

		private readonly List<IModel> _receiverChannels = new List<IModel>();

		public RabbitMqManager(IModelFactory modelFactory)
		{
			_modelFactory = modelFactory;
		}

		public void Publish<TMsg>(string to, TMsg msg)
		{
			using (var channel = _modelFactory.CreateModel())
			{
				channel.ExchangeDeclare("direct_exchange", ExchangeType.Direct);

				var jsonMsg = JsonConvert.SerializeObject(msg);
				var bytesMsg = Encoding.UTF8.GetBytes(jsonMsg);

				var props = channel.CreateBasicProperties();
				props.DeliveryMode = 2;

				channel.BasicPublish(
					exchange: "direct_exchange",
					routingKey: to,
					basicProperties: props,
					body: bytesMsg);
			}
		}

		public void RegisterSubscriber<TMsg>(string from, Action<object, TMsg> eventHandler)
		{
			var receiveChannel = _modelFactory.CreateModel();

			var queueName = receiveChannel.QueueDeclare().QueueName;

			receiveChannel.QueueBind(
				queue: queueName,
				exchange: "direct_exchange",
				routingKey: from);

			var consumer = new EventingBasicConsumer(receiveChannel);

			consumer.Received += (model, ea) =>
			{
				var bytesMsg = ea.Body;
				var jsonMsg = Encoding.UTF8.GetString(bytesMsg);
				var msg = JsonConvert.DeserializeObject<TMsg>(jsonMsg);

				eventHandler(model, msg);
			};

			receiveChannel.BasicConsume(
				queue: queueName,
				noAck: true,
				consumer: consumer);

			_receiverChannels.Add(receiveChannel);
		}

		public void Dispose()
		{
			foreach (var channel in _receiverChannels)
				channel.Close(200, "Goodbye");
		}
	}
}
