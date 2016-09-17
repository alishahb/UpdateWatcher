using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using Alisha.UpdateWatcher.Interfaces;
using Alisha.UpdateWatcher.Managers;
using Alisha.UpdateWatcher.Models;
using NLog;

namespace UpdateWatcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public event EventHandler OnWorkCompleted;
        public event EventHandler OnTick;

        public static ISettings Settings => Config.Settings;
        private static ConfigurationManager<ISettings, SettingsData> Config { get; set; }
        public static DownloadManager DownloadManager { get; set; }
        public static Logger Logger { get; protected set; }
        public static WatcherManager WatcherManager { get; protected set; }


        private void Initialize(object sender, StartupEventArgs startupEventArgs)
        {
            AccessManager.Execute();

            Config = ConfigurationManager<ISettings, SettingsData>.Create();


            if (Config.Settings.CopyItems == null)
                Config.Settings.CopyItems = new ObservableCollection<ICopyItem>();
            if (Config.Settings.RenameFiles == null)
                Config.Settings.RenameFiles = new ObservableCollection<IRenameItem>();


            Config.Settings.DownloadFolder = DirectoryManager.CheckAndCreate(Config.Settings.DownloadFolder, Config.SettingsDirectoryPath, "Download");
            Config.Settings.ExtractFolder = DirectoryManager.CheckAndCreate(Config.Settings.ExtractFolder, Config.SettingsDirectoryPath, "Extract");
            
            Config.Save();

            DownloadManager = new DownloadManager(Config.Settings.DownloadUrl, Config.Settings.DownloadFolder);

            DownloadManager.OnDownloadDataCompleted += OnDownloadDataCompleted;
            DownloadManager.OnDownloadedFileSaved += OnDownloadedFileSaved;
            DownloadManager.OnDownloadStarted += OnDownloadStarted;

            WatcherManager = new WatcherManager(Config.Settings.NextCheckAfter);
            WatcherManager.OnWatchStarted += OnWatchStarted;
            WatcherManager.OnWatchStopped += OnWatchStopped;
            WatcherManager.OnWatchCycleCompleted += OnWatchCycleCompleted;
            WatcherManager.OnTick += OnWatcherTick;

            Logger = LogManager.GetLogger("logfile");
            Logger.Debug("Launched");

            MainWindow = new MainWindow();
            MainWindow.Show();

        }


        #region Tasks

        #region Download

        public void Download()
        {
            Logger.Debug($"DownloadManager :: Checking if new file available to download from url: {Config.Settings.DownloadUrl}");

            DownloadManager.Execute();


        }
        private void OnDownloadStarted(object sender, EventArgs e)
        {
            Logger.Debug($"DownloadManager :: Started checking: {DownloadManager.FileName}");

        }

        private void OnDownloadedFileSaved(object sender, EventArgs e)
        {
            var info = (FileInfo)sender;
            if (DownloadManager.DownloadResult == DownloadResult.Success)
            {
                Config.Settings.LastFileData = new FileData(info);
                Logger.Debug($"DownloadManager :: New file downloaded: {Config.Settings.LastFileData.FileName}, Size: {Config.Settings.LastFileData.FileSize}");

                var factory = new TaskFactory();

                Config.Save();

                factory.StartNew(CleanUp)
                    .ContinueWith(a => Extract())
                    .ContinueWith(a => Copy())
                    .ContinueWith(a => Rename())
                    .ContinueWith(a => Watcher());
            }

        }

        private void OnDownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            Logger.Debug($"DownloadManager :: Check completed, status: {DownloadManager.DownloadResult}");

            var completed = false;

            if (DownloadManager.DownloadResult != DownloadResult.Success)
            {
                if (!Watcher())
                    completed = true;
            }

            if (completed)
            {
                Logger.Debug($"All done, nothing to do");
                OnWorkCompleted?.Invoke(this, null);
            }
        }


        #endregion EndOf-Download

        #region Rename

        private void Rename()
        {
            var renameManager = new RenameManager(Config.Settings.RenameFiles, Config.Settings.ExtractFolder);
            renameManager.OnRenameFile += OnRenameFile;
            renameManager.Execute();

            Logger.Debug($"RenameManager :: Renamed {renameManager.Count} files");

        }
        private void OnRenameFile(string path, string oldName, string newName)
        {
            Logger.Debug($"RenameManager :: Renamed file ({path}): {oldName} -> {newName}");
        }

        #endregion EndOf-Rename

        #region Copy

        private void Copy()
        {
            var copyManager = new CopyManager(Config.Settings.CopyItems, Config.Settings.ExtractFolder);
            copyManager.OnCopyFile += OnCopyFile;
            copyManager.Execute();

            Logger.Debug($"CopyManager :: Copied {copyManager.Count} files");
        }
        private void OnCopyFile(string filename, string copypath)
        {
            Logger.Debug($"CopyManager :: Copied file {filename} -> {copypath}");
        }


        #endregion EndOf-Copy

        private void CleanUp()
        {
            if (!Config.Settings.DeleteExisting) return;

            Logger.Debug($"CleanUpManager :: Checking if we need to cleanup existing directory: {Config.Settings.ExtractFolder}");

            if (Directory.Exists(Config.Settings.ExtractFolder))
            {
                Logger.Debug($"CleanUpManager :: Directory exist, cleaning up");

                var cleanupManager = new CleanupManager(Config.Settings.ExtractFolder);
                cleanupManager.Execute();

                Logger.Debug($"CleanUpManager :: Deleted {cleanupManager.DirectoryCount} Folders, {cleanupManager.FilesCount} Files");
            }
            else Logger.Debug($"CleanUpManager :: No clean up needed");

        }
        private void Extract()
        {

            Logger.Debug($"ExtractManager :: Starting Extraction to: {Config.Settings.ExtractFolder}");

            var extractManager = new ExtractManager(Config.Settings.LastFileData.FullPath, Config.Settings.ExtractFolder);
            var count = extractManager.Execute();

            Logger.Debug($"ExtractManager :: Extracted: {count} files");
        }

        #region Watcher

        private bool Watcher()
        {

            if (!Config.Settings.DaemonMode)
            {
                return false;
            }

            WatcherManager.Execute();
            return true;
        }

        private void OnWatcherTick(object sender, EventArgs e) => OnTick?.Invoke(sender, e);


        private void OnWatchCycleCompleted(object sender, EventArgs e)
        {
            Logger.Debug($"WatcherManager :: daemon cycle completed");
            Download();
        }

        private void OnWatchStopped(object sender, EventArgs e)
        {
            OnWorkCompleted?.Invoke(null, null);

            if (Config.Settings.DaemonMode)
                Logger.Debug($"WatcherManager :: daemon stopped");


        }

        private void OnWatchStarted(object sender, EventArgs e)
        {
            Logger.Debug($"WatcherManager :: starting new daemon cycle to monitor changes, next check after {Config.Settings.NextCheckAfter} minutes");
        }

        public void StopWatch() => WatcherManager?.Cancel();

        #endregion EndOf-Watcher



        #endregion EndOf-Tasks


        public void Save()
        {
            Config.Save();
        }

    }
}
