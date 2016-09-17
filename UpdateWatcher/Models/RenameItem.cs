using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Alisha.UpdateWatcher.Annotations;
using Alisha.UpdateWatcher.Interfaces;

namespace Alisha.UpdateWatcher.Models
{
    [DataContract]
    class RenameItem : IRenameItem, INotifyPropertyChanged
    {

        #region From
        protected string _from;
        [DataMember]
        public string From
        {
            get { return _from; }
            set { _from = value; OnPropertyChanged(); }
        }
        #endregion EndOf-From


        #region To
        protected string _to;
        [DataMember]
        public string To
        {
            get { return _to; }
            set { _to = value; OnPropertyChanged(); }
        }
        #endregion EndOf-To


        #region Extension
        protected string _extension;
        [DataMember]
        public string Extension
        {
            get { return _extension; }
            set { _extension = value; OnPropertyChanged(); }
        }
        #endregion EndOf-Extension


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
