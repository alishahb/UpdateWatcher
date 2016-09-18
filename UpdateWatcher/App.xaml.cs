using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

        public static ISettings Settings => Config.Data;
        public static ObservableCollection<IBuildData> Builds => BuildsInfo.Data.Items;
        public static DownloadManager DownloadManager { get; set; }
        public static Logger Logger { get; protected set; }
        public static WatcherManager WatcherManager { get; protected set; }

        private static ConfigurationManager<ISettings, SettingsData> Config { get; set; }
        private static ConfigurationManager<IBuilds, Builds> BuildsInfo { get; set; }
        private static BuildsManager BuildsManager { get; set; }

        private void Initialize(object sender, StartupEventArgs startupEventArgs)
        {
            AccessManager.Execute();

            Config = ConfigurationManager<ISettings, SettingsData>.Create();
            BuildsInfo = ConfigurationManager<IBuilds, Builds>.Create("Builds");

            Config.Data.DownloadFolder = DirectoryManager.CheckAndCreate(Config.Data.DownloadFolder, Config.SettingsDirectoryPath, "Download");
            Config.Data.ExtractFolder = DirectoryManager.CheckAndCreate(Config.Data.ExtractFolder, Config.SettingsDirectoryPath, "Extract");

            if (Config.Data.CopyItems == null)
                Config.Data.CopyItems = new ObservableCollection<ICopyItem>();
            if (Config.Data.RenameFiles == null)
                Config.Data.RenameFiles = new ObservableCollection<IRenameItem>();

            if (BuildsInfo.Data.Items == null)
                BuildsInfo.Data.Items = new ObservableCollection<IBuildData>();

            if (Config.Data.NextCheckAfter < 1)
                Config.Data.NextCheckAfter = 1;

            Logger = LogManager.GetLogger("logfile");
            Logger.Debug("Launched");

            BuildsManager = new BuildsManager(Builds);
            BuildsManager.OnBuildAdded += OnBuildAdded;
            BuildsManager.OnBuildsAdded += OnBuildsAdded;
            BuildsManager.OnBuildRemoved += OnBuildRemoved;
            BuildsManager.OnBuildsRemoved += OnBuildsRemoved;

            BuildsManager.UpdateInfo(Settings.DownloadFolder);

            if (BuildsManager.LastBuild().FullPath != Settings.LastFileData.FullPath)
                Settings.LastFileData = null;

            RevertBuild();
            Config.Save();

            DownloadManager = new DownloadManager(Config.Data.DownloadFolder);

            DownloadManager.OnDownloadDataCompleted += OnDownloadDataCompleted;
            DownloadManager.OnDownloadedFileSaved += OnDownloadedFileSaved;
            DownloadManager.OnDownloadStarted += OnDownloadStarted;

            WatcherManager = new WatcherManager(Config.Data.NextCheckAfter);
            WatcherManager.OnWatchStarted += OnWatchStarted;
            WatcherManager.OnWatchStopped += OnWatchStopped;
            WatcherManager.OnWatchCycleCompleted += OnWatchCycleCompleted;
            WatcherManager.OnTick += OnWatcherTick;


            MainWindow = new MainWindow();
            MainWindow.Show();



        }

        #region Events
        private void OnBuildAdded(IBuildData info)
        {
            Logger.Debug($"BuildsManager :: New build info was added: {info.FileName}, Size: {info.FileSize} ({info.FullPath})");
        }

        private void OnBuildsAdded(IList<IBuildData> info)
        {
            BuildsInfo.Save();
        }


        private void OnBuildRemoved(IBuildData info, BuildsManager.RemoveReason reason)
        {
            Logger.Debug($"BuildsManager :: [{reason}] for build: {info.FileName}, Size: {info.FileSize} ({info.FullPath}), removing build from list");
        }
        private void OnBuildsRemoved(IList<IBuildData> info)
        {
            BuildsInfo.Save();
        }

        #endregion EndOf-Events

        #region Tasks

        #region Download

        public void Download()
        {
            Logger.Debug($"DownloadManager :: Checking if new file available to download from url: {Config.Data.DownloadUrl}");

            DownloadManager.Execute(Settings.DownloadUrl);


        }
        private void OnDownloadStarted(FileInfo info)
        {
            Logger.Debug($"DownloadManager :: Started checking: {info.Name}");

        }

        private void OnDownloadedFileSaved(FileInfo info)
        {
            if (DownloadManager.DownloadResult == DownloadResult.Failed)
            {
                Logger.Debug($"DownloadManager :: Check Failed: {DownloadManager.ErrorMessage} [{info?.Name ?? "None"}]");
                return;
            }

            if (!info.Exists)
            {
                Logger.Debug($"DownloadManager :: Failed to save file: {info.FullName}");
                return;
            }

            if (DownloadManager.DownloadResult == DownloadResult.NotChanged && Settings.LastFileData == null)
            {
                Settings.LastFileData = new FileData(info);
                Logger.Debug($"LastFileData info is empty, updating to current file {info.Name}");

                Config.Save();

                var factory = new TaskFactory();

                factory.StartNew(CleanUp)
                    .ContinueWith(a => Extract(Config.Data.LastFileData.FullPath))
                    .ContinueWith(a => Copy())
                    .ContinueWith(a => Rename())
                    .ContinueWith(a => Watcher());
            }
            
            else if (DownloadManager.DownloadResult == DownloadResult.Success)
            {
                BuildsManager.UpdateInfo(Settings.DownloadFolder);

                Logger.Debug($"DownloadManager :: New file downloaded: {info.Name}, Size: {info.Length}");

                var factory = new TaskFactory();

                factory.StartNew(() => CheckIgnore(info));
            }
            else Watcher();


        }

        private void OnDownloadDataCompleted(FileInfo info)
        {
            Logger.Debug($"DownloadManager :: Check completed, status: {DownloadManager.DownloadResult}");
        }


        #endregion EndOf-Download


        private void CheckIgnore(FileInfo info)
        {
            if (BuildsManager.IsIgnored(info.FullName))
                Logger.Debug($"CheckIgnore Manager :: This build marked as ignored {info.Name}, Size: {info.Length}, skipping");

            if (!BuildsManager.IsIgnored(info.FullName) && Settings.LastFileData?.FullPath != info.FullName)
            {
                Settings.LastFileData = new FileData(info);

                Logger.Debug($"CheckIgnore Manager :: New build: {info.Name}, Size: {info.Length}");

                Config.Save();

                var factory = new TaskFactory();

                factory.StartNew(CleanUp)
                    .ContinueWith(a => Extract(Config.Data.LastFileData.FullPath))
                    .ContinueWith(a => Copy())
                    .ContinueWith(a => Rename())
                    .ContinueWith(a => Watcher());

                return;
            }

            Save();

        }

        private void CleanUp()
        {
            if (!Config.Data.DeleteExisting) return;

            Logger.Debug($"CleanUpManager :: Checking if we need to cleanup existing directory: {Config.Data.ExtractFolder}");

            if (Directory.Exists(Config.Data.ExtractFolder))
            {
                Logger.Debug($"CleanUpManager :: Directory exist, cleaning up");

                var cleanupManager = new CleanupManager(Config.Data.ExtractFolder);
                cleanupManager.Execute();

                Logger.Debug($"CleanUpManager :: Deleted {cleanupManager.DirectoryCount} Folders, {cleanupManager.FilesCount} Files");
            }
            else Logger.Debug($"CleanUpManager :: No clean up needed");

        }
        private void Extract(string path)
        {

            Logger.Debug($"ExtractManager :: Starting Extraction to: {Config.Data.ExtractFolder}");

            var extractManager = new ExtractManager(path, Config.Data.ExtractFolder);
            var count = extractManager.Execute();

            Logger.Debug($"ExtractManager :: Extracted: {count} files");
        }

        #region Copy

        private void Copy()
        {
            var copyManager = new CopyManager(Config.Data.CopyItems, Config.Data.ExtractFolder);
            copyManager.OnCopyFile += OnCopyFile;
            copyManager.Execute();

            Logger.Debug($"CopyManager :: Copied {copyManager.Count} files");
        }
        private void OnCopyFile(string filename, string copypath)
        {
            Logger.Debug($"CopyManager :: Copied file {filename} -> {copypath}");
        }


        #endregion EndOf-Copy

        #region Rename

        private void Rename()
        {
            var renameManager = new RenameManager(Config.Data.RenameFiles, Config.Data.ExtractFolder);
            renameManager.OnRenameFile += OnRenameFile;
            renameManager.Execute();

            Logger.Debug($"RenameManager :: Renamed {renameManager.Count} files");

        }
        private void OnRenameFile(string path, string oldName, string newName)
        {
            Logger.Debug($"RenameManager :: Renamed file ({path}): {oldName} -> {newName}");
        }

        #endregion EndOf-Rename



        #region Watcher

        private bool Watcher()
        {

            if (!Config.Data.DaemonMode)
            {
                Logger.Debug($"All done, nothing to do ({DownloadManager.DownloadResult})");
                OnWorkCompleted?.Invoke(this, null);
                return false;
            }

            WatcherManager.Execute();
            return true;
        }

        private void OnWatcherTick(object sender, EventArgs e)
        {
            OnTick?.Invoke(sender, e);
        }


        private void OnWatchCycleCompleted(object sender, EventArgs e)
        {
            Logger.Debug($"WatcherManager :: Daemon cycle completed #{((WatcherManager)sender).CycleNumber}, next check after {Config.Data.NextCheckAfter} minutes");
            Download();
        }

        private void OnWatchStopped(object sender, EventArgs e)
        {
            OnWorkCompleted?.Invoke(null, null);

            if (Config.Data.DaemonMode)
                Logger.Debug($"WatcherManager :: Daemon stopped");


        }

        private void OnWatchStarted(object sender, EventArgs e)
        {
            Logger.Debug($"WatcherManager :: Starting new daemon to monitor changes, next check after {Config.Data.NextCheckAfter} minutes");
        }

        public void StopWatch() => WatcherManager?.Cancel();

        #endregion EndOf-Watcher



        #endregion EndOf-Tasks


        public void Save()
        {
            RevertBuild();
            BuildsManager.UpdateInfo(Settings.DownloadFolder);

            Config.Save();
            BuildsInfo.Save();

        }

        private void RevertBuild()
        {
            if (Settings.LastFileData == null) return;
            if (BuildsManager.IsIgnored(Settings.LastFileData.FullPath))
            {
                Logger.Debug($"RevertBuild Manager :: Current build [{Settings.LastFileData.FileName}] marked as ignored, will try revert previous if have one");

                var build = Builds.Where(b => !b.Ignore).Select(b => new FileInfo(b.FullPath)).OrderByDescending(b => b.CreationTimeUtc).FirstOrDefault();

                if (build == null)
                {
                    Logger.Debug($"RevertBuild Manager :: No early builds, which are not ignored found");
                    return;
                }

                Logger.Debug($"RevertBuild Manager :: Found build to revert: {build.Name}, {build.Length} downloaded on {build.CreationTime.ToLongDateString()} {build.CreationTime.ToLongTimeString()}");

                var factory = new TaskFactory();

                factory.StartNew(CleanUp)
                   .ContinueWith(a => Extract(build.FullName))
                   .ContinueWith(a => Copy())
                   .ContinueWith(a => Rename());

                Settings.LastFileData = new FileData(build);

                Logger.Debug($"RevertBuild Manager :: Version reverted to build [{Settings.LastFileData.FileName}]");
            }
        }
    }
}
