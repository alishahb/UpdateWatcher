using System.ComponentModel;
using System.Runtime.Serialization;
using Alisha;

namespace Alisha.UpdateWatcher.Interfaces
{
    public interface ICopyItem
    {
        [DataMember(EmitDefaultValue = true)]
        [DefaultValue(default(string))]
        string Pattern { get; set; }
        [DataMember(EmitDefaultValue = true)]
        [DefaultValue(default(string))]
        string Path { get; set; }
        [DataMember(EmitDefaultValue = true)]
        [DefaultValue(false)]
        bool Recursive { get; set; }

    }
}