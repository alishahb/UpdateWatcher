using System.ComponentModel;
using System.Runtime.Serialization;
using Alisha;

namespace Alisha.UpdateWatcher.Interfaces
{
    public interface IFileData : IInterface
    {
        [DataMember(EmitDefaultValue = true)]
        [DefaultValue(0)]
        long FileSize { get; set; }

        [DataMember(EmitDefaultValue = true)]
        [DefaultValue(typeof(string))]
        string FileName { get; set; }


        [DataMember(EmitDefaultValue = true)]
        [DefaultValue(typeof(string))]
        string FullPath { get; set; }

    }
}