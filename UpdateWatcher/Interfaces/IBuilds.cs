using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Alisha.UpdateWatcher.Interfaces
{
    public interface IBuilds : IInterface
    {
        [DataMember(EmitDefaultValue = true)]
        [DefaultValue(default(ObservableCollection<IBuildData>))]
        ObservableCollection<IBuildData> Items { get; set; }
    }
}
