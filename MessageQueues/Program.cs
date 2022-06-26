using System;
using RabbitMQ.Client;
using static Common.RabbitMqConfiguration;

namespace DataProcessing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter output folder: ");
            var folderPath = Console.ReadLine();
            var filePath = $@"..\{folderPath}";

            var factory = new ConnectionFactory
            {
                Uri = new Uri(Url)
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            var processingService = new FileProcessingRabbitMqService(filePath, channel);
            Console.ReadLine();

            connection.Close();
            channel.Close();
        }
    }
}
