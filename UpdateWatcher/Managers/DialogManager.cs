using System;
using System.IO;
using System.Windows.Forms;

namespace Alisha.UpdateWatcher.Managers
{
    class DialogManager
    {
        public static string OpenDialog(string dependedObject)
        {
            var value = dependedObject;

            var dlg = new FolderBrowserDialog { SelectedPath = Environment.CurrentDirectory };

            try
            {
                if (new DirectoryInfo(value).Exists)
                    dlg.SelectedPath = value;
            }
            catch (Exception)
            {
            }


            if (dlg.ShowDialog() == DialogResult.OK)
            {
                value = dlg.SelectedPath;
            }

            return value;
        }

    }
}
