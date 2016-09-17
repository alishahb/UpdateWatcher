using System.IO;

namespace Alisha.UpdateWatcher.Managers
{
    internal class LogWatcher : FileSystemWatcher
    {
        internal string FileName;

        FileStream Stream;
        StreamReader Reader;

        public LogWatcher(string FileName)
        {
            this.Changed += OnChanged;

            this.FileName = FileName;
            var directory = new FileInfo(FileName).DirectoryName;

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            Stream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            Reader = new StreamReader(Stream);

            Stream.Position = Stream.Length;
        }

        public void OnChanged(object o, FileSystemEventArgs e)
        {
            string Contents = Reader.ReadToEnd();

            LogWatcherEventArgs args = new LogWatcherEventArgs(Contents);
            TextChanged?.Invoke(this, args);
        }

        public delegate void LogWatcherEventHandler(object sender, LogWatcherEventArgs e);

        public event LogWatcherEventHandler TextChanged;
    }

    internal class LogWatcherEventArgs
    {
        public string Contents { get; set; }

        public LogWatcherEventArgs(string contents)
        {
            Contents = contents;
        }
    }
}
