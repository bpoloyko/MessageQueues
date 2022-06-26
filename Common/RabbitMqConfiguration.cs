using System;

namespace Common
{
    public static class RabbitMqConfiguration
    {
        public const string Exchange = "fileServiceExchange";

        public const string ProcessingQueue = "processingQueue";

        public const string ProcessRoutingKey = "fileService.process";

        public const string Url = @"amqp://guest:guest@localhost:5672";
    }
}
