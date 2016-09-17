using System.Collections.Generic;
using System.IO;
using System.Linq;
using Alisha.UpdateWatcher.Interfaces;

namespace Alisha.UpdateWatcher.Managers
{

    public delegate void OnCopyFile(string fileName, string copyPath);
    class CopyManager
    {
        public OnCopyFile OnCopyFile;

        public IList<ICopyItem> CopyData { get; }
        public string FolderPath { get; }
        public uint Count { get; protected set; }

        public CopyManager(IList<ICopyItem> copyData, string folderPath)
        {
            FolderPath = folderPath;
            CopyData = copyData;
        }


        public void Execute()
        {
            if (CopyData == null || CopyData.Count == 0) return;
            if (!Directory.Exists(FolderPath)) return;


            Count = 0;
            foreach (var data in CopyData)
            {
                if (!Directory.Exists(data.Path))
                    Directory.CreateDirectory(data.Path);

                foreach (var file in Directory.GetFiles(FolderPath, data.Pattern, data.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).Select(f => new FileInfo(f)))
                {
                    var newPath = Path.Combine(data.Path, file.Name);

                    File.Copy(file.FullName, newPath, true);
                    File.SetAttributes(newPath, FileAttributes.Normal);

                    OnCopyFile?.Invoke(file.FullName, newPath);
                    Count++;
                }
            }
        }
    }
}
