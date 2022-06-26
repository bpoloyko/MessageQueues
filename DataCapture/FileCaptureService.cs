using System.IO;

namespace DataCapture
{
    public abstract class FileCaptureService
    {
        protected FileCaptureService(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var watcher = new FileSystemWatcher(path) { EnableRaisingEvents = true };
            watcher.Created += OnCreated;
        }

        protected abstract void OnCreated(object sender, FileSystemEventArgs e);
    }
}
