using System;
using System.IO;
using SevenZip;

namespace Alisha.UpdateWatcher.Managers
{
    class ExtractManager
    {
        public FileInfo FilePath { get; }
        public string ExtractDirectory { get; }
        public Guid Guid { get; }

        public ExtractManager(string filePath, string extractDirectory)
        {
            FilePath = new FileInfo(filePath);
            ExtractDirectory = extractDirectory;
            Guid = Guid.NewGuid();
        }


        public uint Execute()
        {
            SevenZipBase.SetLibraryPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z.dll"));


            using (SevenZipExtractor zip = new SevenZipExtractor(FilePath.FullName))
            {
                zip.ExtractArchive(ExtractDirectory);
                var count = zip.FilesCount;
                zip.Dispose();
                return count;
            }

        }

    }
}
