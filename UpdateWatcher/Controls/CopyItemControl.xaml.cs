using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Alisha.UpdateWatcher.Interfaces;
using Alisha.UpdateWatcher.Models;
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
            CopyItem.Path = OpenDialog(CopyItem.Path);
        }

        private string OpenDialog(string dependedObject)
        {
            var value = dependedObject;

            var dlg = new FolderBrowserDialog { SelectedPath = new DirectoryInfo(dependedObject).FullName };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                value = dlg.SelectedPath;
            }

            return value;
        }
    }
}
