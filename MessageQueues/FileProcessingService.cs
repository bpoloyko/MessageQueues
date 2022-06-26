using RabbitMQ.Client.Events;


namespace DataProcessing
{
    public abstract class FileProcessingService
    {
        
        protected readonly string _path;

        protected FileProcessingService(string path)
        {
            _path = path;
        }

        protected abstract void OnReceived(object sender, BasicDeliverEventArgs args);
    }
}
