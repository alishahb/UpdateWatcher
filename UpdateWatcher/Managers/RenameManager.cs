using System.Collections.Generic;
using System.IO;
using System.Linq;
using Alisha.UpdateWatcher.Interfaces;

namespace Alisha.UpdateWatcher.Managers
{
    public delegate void OnRenameFile(string path, string oldName, string newName);

    class RenameManager
    {
        public OnRenameFile OnRenameFile;

        public IList<IRenameItem> Items { get; }
        public string Path { get; }
        public uint Count { get; protected set; }

        public RenameManager(IList<IRenameItem> items, string path)
        {
            Items = items;
            Path = path;
        }

        public void Execute()
        {
            if (Items == null || Items.Count == 0) return;
            if (!Directory.Exists(Path)) return;

            Count = 0;
            foreach (var file in Directory.GetFiles(Path).Select(s => new FileInfo(s)))
            {
                foreach (var item in Items)
                {
                    if (file.Name.Contains(item.From) && item.Extension == file.Extension)
                    {
                        var newName = file.Name.Replace(item.From, item.To);

                        if (newName == file.Name) continue;

                        var newPath = System.IO.Path.Combine(file.DirectoryName, newName);

                        if (File.Exists(newPath))
                            File.Delete(newPath);

                        File.Move(file.FullName, newPath);
                        OnRenameFile?.Invoke(file.DirectoryName, file.Name, newName);

                        Count++;
                    }
                }

            }
        }
    }
}
