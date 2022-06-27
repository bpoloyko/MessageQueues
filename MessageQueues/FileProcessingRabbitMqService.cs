using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using static Common.RabbitMqConfiguration;

namespace DataProcessing
{
    public class FileProcessingRabbitMqService : FileProcessingService
    {
        private readonly ConcurrentDictionary<string, List<Chunk>> _data = new();

        public FileProcessingRabbitMqService(string path, IModel channel) : base(path)
        {
            channel.ExchangeDeclare(Exchange, ExchangeType.Direct, true);
            channel.QueueDeclare(ProcessingQueue, true, false, false);
            channel.QueueBind(ProcessingQueue, Exchange, ProcessRoutingKey);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnReceived;
            channel.BasicConsume(ProcessingQueue, true, consumer);
        }

        protected override void OnReceived(object sender, BasicDeliverEventArgs args)
        {
            var size = (int) args.BasicProperties.Headers["size"];
            var position = (int)args.BasicProperties.Headers["position"];
            _data.GetOrAdd(args.BasicProperties.MessageId, new List<Chunk>());

            var chunk = new Chunk
            {
                Body = args.Body.ToArray(),
                Position = position,
                Size = size
            };

            _data[args.BasicProperties.MessageId].Add(chunk);

            if (_data[args.BasicProperties.MessageId].Count == size)
            {
                var fileNameBytes = (byte[])args.BasicProperties.Headers["fileName"];
                var fileName = Encoding.UTF8.GetString(fileNameBytes);
                Console.WriteLine($"{fileName} received");
                var bytes = new List<byte>();
                foreach (var messageChunk in _data[args.BasicProperties.MessageId].OrderBy(x => x.Position))
                {
                    bytes.AddRange(messageChunk.Body);
                }

                File.WriteAllBytes($"{_path}\\{fileName}", bytes.ToArray());
            }
        }
    }
}
