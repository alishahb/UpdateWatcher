using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Alisha;
using Alisha.UpdateWatcher.Annotations;

namespace Alisha.UpdateWatcher.Models
{
    [DataContract]
    class DataModel : INotifyPropertyChanged
    {
        public static readonly Regex NumberCheckRegex = new Regex("[0-9]+");

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