using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common;
using Polly;
using RabbitMQ.Client;
using static Common.RabbitMqConfiguration;

namespace DataCapture
{
    public class FileCaptureRabbitMqService : FileCaptureService
    {
        private readonly IModel _channel;
        private readonly string[] _allowedFileExtensions;

        public FileCaptureRabbitMqService(IModel channel, string path, string[] allowedFileExtensions) : base(path)
        {
            _channel = channel;
            _channel.ExchangeDeclare(Exchange, ExchangeType.Direct, true);
            _allowedFileExtensions = allowedFileExtensions;
        }

        protected override async void OnCreated(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Created)
            {
                return;
            }

            var extension = Path.GetExtension(e.FullPath);
            if (!_allowedFileExtensions.Contains(extension))
            {
                Console.WriteLine($"Invalid file extension received: {extension}");
                return;
            }

            var retryPolicy = Policy
                .Handle<IOException>()
                .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(3));

            await retryPolicy.ExecuteAsync(async () =>
            {
                var bytes = await File.ReadAllBytesAsync(e.FullPath);
                var chunks = bytes.GetChunks();
                SendMessage(e.Name, chunks, ProcessRoutingKey);
                Console.WriteLine($"{e.Name} was sent");
            });
        }

        private void SendMessage(string fileName, IEnumerable<Chunk> message, string routingKey)
        {
            var properties = _channel.CreateBasicProperties();
            properties.Headers = new Dictionary<string, object> { { "fileName", fileName }};
            foreach (var chunk in message)
            {
                properties.MessageId = chunk.Id.ToString();
                properties.Headers["size"] = chunk.Size;
                properties.Headers["position"] = chunk.Position;
                _channel.BasicPublish(Exchange, routingKey, properties, chunk.Body);
            }
        }
    }
}
