using System.IO;
using System.Runtime.Serialization;
using Alisha;
using Alisha.UpdateWatcher.Interfaces;

namespace Alisha.UpdateWatcher.Models
{
    [DataContract]
    class FileData : IFileData
    {
        public FileData()
        {
            
        }
        public FileData(FileInfo info)
        {
            FileName = info.Name;
            FullPath = info.FullName;
            FileSize = info.Length;
        }

        [DataMember]
        public string FullPath { get; set; }
        [DataMember]
        public long FileSize { get; set; }
        [DataMember]
        public string FileName { get; set; }
    }
}