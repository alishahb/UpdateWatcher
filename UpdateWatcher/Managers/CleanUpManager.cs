using System;
using System.IO;
using System.Threading;

namespace Alisha.UpdateWatcher.Managers
{
    class CleanupManager
    {
        public string Path { get; }
        public int FilesCount { get; protected set; }
        public int DirectoryCount { get; protected set; }

        public CleanupManager(string path)
        {
            Path = path;

        }

        public void Execute()
        {
            FilesCount = 0;
            DirectoryCount = 0;
            Execute(Path);
        }

        private void Execute(string path)
        {

            Thread.Sleep(50);

            string[] files = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);

            foreach (string file in files)
            {
                if (File.Exists(file))
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                    FilesCount++;
                }
            }

            foreach (string dir in dirs)
            {
                Execute(dir);
            }

            DirectoryCount++;
            DeleteDirectory(path);
        }

        public static void DeleteDirectory(string path, bool recursive = false)
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                DeleteDirectory(directory);
            }

            try
            {
                Directory.Delete(path, recursive);
            }
            catch (IOException)
            {
                Directory.Delete(path, recursive);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, recursive);
            }
        }
    }
}
