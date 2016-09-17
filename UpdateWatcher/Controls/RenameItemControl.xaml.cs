using Alisha.UpdateWatcher.Interfaces;
using UserControl = System.Windows.Controls.UserControl;

namespace Alisha.UpdateWatcher.Controls
{
    /// <summary>
    /// Interaction logic for RenameItemControl.xaml
    /// </summary>
    public partial class RenameItemControl : UserControl
    {
        public IRenameItem Item { get; set; }

        public RenameItemControl(IRenameItem item)
        {
            Item = item;

            InitializeComponent();
        }

    }
}
