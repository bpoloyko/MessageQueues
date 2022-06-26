using System;
using RabbitMQ.Client;
using static Common.RabbitMqConfiguration;

namespace DataCapture
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter input folder: ");
            var folderPath = Console.ReadLine();
            var filePath = $@"..\{folderPath}";

            var factory = new ConnectionFactory
            {
                Uri = new Uri(Url)
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            var fileService = new FileCaptureRabbitMqService(channel, filePath, new[] { ".pdf", ".mp4" });
            Console.ReadLine();
            connection.Close();
            channel.Close();
        }
    }
}
