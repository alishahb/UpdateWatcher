using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Alisha;
using Alisha.UpdateWatcher.Interfaces;

namespace Alisha.UpdateWatcher.Models
{
    class Builds : DataModel, IBuilds
    {

        #region Builds
        protected ObservableCollection<IBuildData> _builds;
        [DataMember]
        public ObservableCollection<IBuildData> Items
        {
            get { return _builds; }
            set { _builds = value; OnPropertyChanged(); }
        }
        #endregion EndOf-Builds

    }
}