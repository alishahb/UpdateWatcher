using System.IO;

namespace Alisha.UpdateWatcher.Managers
{
    class DirectoryManager
    {
        public static string CheckAndCreate(string path, string basepath, string name)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = Path.Combine(basepath, name);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
    }
}
