using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Alisha;
using Alisha.UpdateWatcher.Annotations;
using Alisha.UpdateWatcher.Converter;
using Alisha.UpdateWatcher.Interfaces;
using Newtonsoft.Json;

namespace Alisha.UpdateWatcher.Models
{
    [DataContract]
    class SettingsData : DataModel, ISettings, INotifyPropertyChanged
    {

        [DataMember]
        [JsonConverter(typeof(InterfaceListConverter<ICopyItem, CopyItem>))]
        public ObservableCollection<ICopyItem> CopyItems { get; set; }
        [DataMember]
        [JsonConverter(typeof(InterfaceListConverter<IRenameItem, RenameItem>))]
        public ObservableCollection<IRenameItem> RenameFiles { get; set; }



        #region DownloadFolder
        protected string _downloadFolder;
        [DataMember]
        public string DownloadFolder
        {
            get { return _downloadFolder; }
            set { _downloadFolder = value; OnPropertyChanged(); }
        }
        #endregion EndOf-DownloadFolder

        [DataMember]
        [JsonConverter(typeof(InterfaceConverter<IFileData, FileData>))]
        public IFileData LastFileData { get; set; }


        #region DownloadUrl
        protected string _downloadUrl;
        [DataMember]
        public string DownloadUrl
        {
            get { return _downloadUrl; }
            set { _downloadUrl = value; OnPropertyChanged(); }
        }
        #endregion EndOf-DownloadUrl


        #region ExtractFolder
        protected string _extractFolder;
        [DataMember]
        public string ExtractFolder
        {
            get { return _extractFolder; }
            set { _extractFolder = value; OnPropertyChanged(); }
        }
        #endregion EndOf-ExtractFolder


        #region MinimizeToTray
        protected bool _minimizeToTray;
        [DataMember]
        public bool MinimizeToTray
        {
            get { return _minimizeToTray; }
            set { _minimizeToTray = value; OnPropertyChanged(); }
        }
        #endregion EndOf-MinimizeToTray

        #region DeleteExisting
        protected bool _deleteExisting;
        [DataMember]
        public bool DeleteExisting
        {
            get { return _deleteExisting; }
            set { _deleteExisting = value; OnPropertyChanged(); }
        }
        #endregion EndOf-PropertyName

        #region DaemonMode

        protected bool _daemonMode;
        [DataMember]
        public bool DaemonMode
        {
            get { return _daemonMode; }
            set { _daemonMode = value; OnPropertyChanged(); }
        }
        #endregion EndOf-PropertyName

        #region NextCheckAfter

        protected int _nextCheckAfter;
        [DataMember]
        public int NextCheckAfter
        {
            get { return _nextCheckAfter; }
            set {
                // Preventing setting value less then 1 minute
                _nextCheckAfter = Math.Max(NumberCheckRegex.IsMatch(value.ToString()) ? value : 1, 1);
                OnPropertyChanged();
            }
        }

        #endregion EndOf-NextCheckAfter


        #region IPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion EndOf-IPropertyChanged

    }
}