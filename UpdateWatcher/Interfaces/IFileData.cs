using System.ComponentModel;
using System.Runtime.Serialization;
using Alisha;

namespace Alisha.UpdateWatcher.Interfaces
{
    public interface IFileData
    {
        [DataMember(EmitDefaultValue = true)]
        [DefaultValue(0)]
        long FileSize { get; }

        [DataMember(EmitDefaultValue = true)]
        [DefaultValue(typeof(string))]
        string FileName { get; }


        [DataMember(EmitDefaultValue = true)]
        [DefaultValue(typeof(string))]
        string FullPath { get; set; }

    }
}