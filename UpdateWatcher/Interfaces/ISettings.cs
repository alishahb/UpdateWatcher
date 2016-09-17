using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Alisha.UpdateWatcher.Interfaces
{

    public interface ISettings : IInterface
    {
        [DataMember(EmitDefaultValue = true)]
        [DefaultValue(typeof(string))]
        string DownloadUrl { get; set; }

        [DataMember(EmitDefaultValue = true)]
        [DefaultValue("Download")]
        string DownloadFolder { get; set; }


        [DataMember(EmitDefaultValue = true)]
        [DefaultValue(typeof(string))]
        string ExtractFolder { get; set; }

        [DataMember(EmitDefaultValue = true)]
        [DefaultValue(typeof(string))]
        IFileData LastFileData { get; set; }

        [DataMember(EmitDefaultValue = true)]
        [DefaultValue(typeof(string))]
        ObservableCollection<IRenameItem> RenameFiles { get; set; }

        [DataMember(EmitDefaultValue = true)]
        [DefaultValue(typeof(string))]
        ObservableCollection<ICopyItem> CopyItems { get; set; }

        [DataMember(EmitDefaultValue = true)]
        [DefaultValue(true)]
        bool MinimizeToTray { get; set; }

        [DataMember(EmitDefaultValue = true)]
        [DefaultValue(false)]
        bool DeleteExisting { get; set; }

        [DataMember(EmitDefaultValue = true)]
        [DefaultValue(true)]
        bool DaemonMode { get; set; }

        [DataMember(EmitDefaultValue = true)]
        [DefaultValue(5)]
        int NextCheckAfter { get; set; }

    }
}
