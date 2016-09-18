using System.Runtime.Serialization;
using Alisha.UpdateWatcher.Interfaces;

namespace Alisha.UpdateWatcher.Models
{
    class BuildData : DataModel, IBuildData
    {

        #region FileSize

        protected long _fileSize;

        [DataMember]
        public long FileSize
        {
            get { return _fileSize; }
            set {
                if (NumberCheckRegex.IsMatch(value.ToString())) _fileSize = value;
                OnPropertyChanged();
            }
        }

        #endregion EndOf-FileSize


        #region FileName
        protected string _fileName;
        [DataMember]
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; OnPropertyChanged(); }
        }
        #endregion EndOf-FileName


        #region FullPath
        protected string _fullPath;
        [DataMember]
        public string FullPath
        {
            get { return _fullPath; }
            set { _fullPath = value; OnPropertyChanged(); }
        }
        #endregion EndOf-FullPath


        #region Ignore
        protected bool _ignore;
        [DataMember]
        public bool Ignore
        {
            get { return _ignore; }
            set { _ignore = value; OnPropertyChanged(); }
        }
        #endregion EndOf-Ignore

    }
}
