using System.Windows;
using Alisha.UpdateWatcher.Interfaces;
using Alisha.UpdateWatcher.Managers;
using UserControl = System.Windows.Controls.UserControl;

namespace Alisha.UpdateWatcher.Controls
{
    /// <summary>
    /// Interaction logic for CopyItemControl.xaml
    /// </summary>
    public partial class CopyItemControl : UserControl
    {
        public ICopyItem CopyItem { get; set; }


        public CopyItemControl(ICopyItem copyItem)
        {
            CopyItem = copyItem;
            InitializeComponent();
        }

        private void Browse(object sender, RoutedEventArgs e)
        {
            CopyItem.Path = DialogManager.OpenDialog(CopyItem.Path);
        }

    }
}
