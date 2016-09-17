using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Alisha;
using Alisha.UpdateWatcher.Annotations;
using Alisha.UpdateWatcher.Interfaces;

namespace Alisha.UpdateWatcher.Models
{
    [DataContract]
    public class CopyItem : ICopyItem, INotifyPropertyChanged
    {

        #region Pattern
        protected string _pattern;
        [DataMember]
        public string Pattern
        {
            get { return _pattern; }
            set { _pattern = value; OnPropertyChanged(); }
        }
        #endregion EndOf-Pattern

        #region Path
        protected string _path;
        [DataMember]
        public string Path
        {
            get { return _path; }
            set { _path = value; OnPropertyChanged(); }
        }
        #endregion EndOf-Path


        #region Recursive
        protected bool _recursive;
        [DataMember]
        public bool Recursive
        {
            get { return _recursive; }
            set { _recursive = value; OnPropertyChanged(); }
        }
        #endregion EndOf-Recursive


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion EndOf-INotifyPropertyChanged


    }
}