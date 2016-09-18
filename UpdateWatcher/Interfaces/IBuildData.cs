using System.ComponentModel;
using System.Runtime.Serialization;
using Alisha;

namespace Alisha.UpdateWatcher.Interfaces
{
    public interface IBuildData : IFileData
    {
        [DataMember(EmitDefaultValue = true)]
        [DefaultValue(false)]
        bool Ignore { get; set; }
    }
}