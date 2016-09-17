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
