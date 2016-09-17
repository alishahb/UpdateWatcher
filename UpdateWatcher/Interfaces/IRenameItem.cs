using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Alisha;

namespace Alisha.UpdateWatcher.Interfaces
{
    public interface IRenameItem
    {
        [DataMember(EmitDefaultValue = true)]
        [DefaultValue(typeof(string))]
        string From { get; set; }

        [DataMember(EmitDefaultValue = true)]
        [DefaultValue(typeof(string))]
        string To { get; set; }

        [DataMember(EmitDefaultValue = true)]
        [DefaultValue(typeof(string))]
        string Extension { get; set; }
    }
}