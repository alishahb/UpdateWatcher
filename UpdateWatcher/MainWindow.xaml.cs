using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Alisha.UpdateWatcher.Annotations;
using Alisha.UpdateWatcher.Controls;
using Alisha.UpdateWatcher.Interfaces;
using Alisha.UpdateWatcher.Managers;
using Alisha.UpdateWatcher.Models;
using Newtonsoft.Json;
using NLog;
using NLog.Targets;
using Application = System.Windows.Application;

namespace UpdateWatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        internal LogWatcher _logWatcher;
        private NotifyIcon _ni;
        public ISettings Settings => App.Settings;
        public ObservableCollection<IBuildData> Builds => App.Builds;


        public DownloadManager DownloadManager => App.DownloadManager;


        #region CopyItems
        protected ObservableCollection<CopyItemControl> _copyItems;
        [DataMember]
        public ObservableCollection<CopyItemControl> CopyItems
        {
            get { return _copyItems; }
            set { _copyItems = value; OnPropertyChanged(); }
        }
        #endregion EndOf-CopyItems


        #region RenameItems
        protected ObservableCollection<RenameItemControl> _renameItems;
        [DataMember]
        [DefaultValue(typeof(ObservableCollection<RenameItemControl>))]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public ObservableCollection<RenameItemControl> RenameItems
        {
            get { return _renameItems; }
            set { _renameItems = value; OnPropertyChanged(); }
        }
        #endregion EndOf-RenameItems



        #region Idle
        protected bool _idle;
        public bool Idle
        {
            get { return _idle; }
            set { _idle = value; OnPropertyChanged(); }
        }
        #endregion EndOf-Idle

        public MainWindow()
        {
            InitializeComponent();
            SetupContainers();


            Idle = true;

            _ni = new System.Windows.Forms.NotifyIcon
            {
                Icon = Alisha.UpdateWatcher.Properties.Resources.eye_ico,
                Visible = true
            };

            _ni.DoubleClick += delegate
            {
                Show();
                WindowState = WindowState.Normal;
            };

            Application.Current.Exit += Current_Exit;

            ((App)App.Current).OnWorkCompleted += OnWorkCompleted;
            ((App)App.Current).OnWorkStarted += OnWorkStarted;
            ((App)App.Current).OnTick += OnTick;
            ((App)App.Current).OnTick += OnTick;

            App.DownloadManager.OnDownloadProgressChanged += OnDownloadProgressChanged;

            ResetState();
            WatcherTimer.Visibility = Visibility.Collapsed;

            var data = Assembly.GetExecutingAssembly().GetName();
            Title = $"{Assembly.GetExecutingAssembly().GetName().Name} v{data.Version} by Alisha";

            CreateLogWatcher(GetLogFile());
        }



        private void SetupContainers()
        {
            CopyItems = new ObservableCollection<CopyItemControl>();

            CopyItemsContainer.ItemsSource = CopyItems;

            foreach (var item in Settings.CopyItems)
            {
                CopyItems.Add(new CopyItemControl(item));
            }

            RenameItems = new ObservableCollection<RenameItemControl>();

            RenameItemsContainer.ItemsSource = RenameItems;

            foreach (var item in Settings.RenameFiles)
            {
                RenameItems.Add(new RenameItemControl(item));
            }

        }



        private void OnTick(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                OnStarted();
                WatcherTimer.Visibility = Visibility.Visible;
                WatcherTimer.Content = ((TimeSpan)sender).ToString("h'h 'm'm 's's'");
            });
        }


        private string GetLogFile()
        {
            var fileTarget = LogManager.Configuration.AllTargets.FirstOrDefault(t => t is FileTarget) as FileTarget;
            return fileTarget == null ? string.Empty : fileTarget.FileName.Render(new LogEventInfo { Level = LogLevel.Debug });
        }

        private void BtnStart_OnClick(object sender, RoutedEventArgs e)
        {
            OnStarted();

            if (Settings.MinimizeToTray)
                Hide();

            ((App)App.Current).Download();

        }
        private void OnWorkStarted(object sender, EventArgs e)
        {
            OnStarted();
        }


        private void OnStarted()
        {
            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;
            Idle = false;
            DownlloadProgressBar.Visibility = Visibility.Visible;
        }

        private void OnWorkCompleted(object sender, EventArgs e)
        {
            if (!App.Settings.DaemonMode)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ResetState();
                    DownlloadProgressBar.Visibility = Visibility.Collapsed;
                    WatcherTimer.Visibility = Visibility.Hidden;
                });
            }

        }

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => DownlloadProgressBar.Value = e.ProgressPercentage);
        }


        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                this.Hide();

            base.OnStateChanged(e);
        }


        void Current_Exit(object sender, ExitEventArgs e) => _ni.Dispose();

        private void CreateLogWatcher(string FileName)
        {
            _logWatcher = new LogWatcher(FileName);
            _logWatcher.Path = Path.GetDirectoryName(FileName);
            _logWatcher.NotifyFilter = (NotifyFilters.LastWrite | NotifyFilters.Size);
            _logWatcher.Filter = Path.GetFileName(FileName);
            _logWatcher.TextChanged += new LogWatcher.LogWatcherEventHandler(Watcher_Changed);
            _logWatcher.EnableRaisingEvents = true;

            //AppendText(FileName);
        }


        void Watcher_Changed(object sender, LogWatcherEventArgs e) =>
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                AppendText(e.Contents);
            }));

        private void AppendText(string data)
        {
            LogBox.Text += data;
            LogBox.ScrollToEnd();

        }

        private void BtnStop_OnClick(object sender, RoutedEventArgs e)
        {
            ((App)App.Current).StopWatch();
            ResetState();
        }

        private void ResetState()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                DownlloadProgressBar.Visibility = Visibility.Collapsed;
                WatcherTimer.Visibility = Visibility.Collapsed;
                btnStop.IsEnabled = false;
                btnStart.IsEnabled = true;
                Idle = true;
                WatcherTimer.Content = TimeSpan.FromMinutes(0).ToString("h'h 'm'm 's's'");
            });

        }



        private void BtnDrowseDownloadFolder_OnClick(object sender, RoutedEventArgs e)
        {
            Settings.DownloadFolder = DialogManager.OpenDialog(Settings.DownloadFolder);
        }


        private void BtnDrowseExtractFolder_OnClick(object sender, RoutedEventArgs e)
        {
            Settings.ExtractFolder = DialogManager.OpenDialog(Settings.ExtractFolder);
        }


        #region IPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion EndOf-IPropertyChanged

        private void addRenameItem(object sender, RoutedEventArgs e)
        {
            var item = new RenameItem();
            Settings.RenameFiles.Add(item);
            RenameItems.Add(new RenameItemControl(item));
        }

        private void addCopyItem(object sender, RoutedEventArgs e)
        {
            var item = new CopyItem();
            Settings.CopyItems.Add(item);
            CopyItems.Add(new CopyItemControl(item));
        }

        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            ((App)App.Current).Save();
        }

        private void deleteCopyItem(object sender, RoutedEventArgs e)
        {
            if (CopyItemsContainer.SelectedIndex < 0) return;

            var selected = (CopyItemControl)CopyItemsContainer.SelectedItem;

            Settings.CopyItems.Remove(selected.CopyItem);
            CopyItems.Remove(selected);
        }

        private void deleteRenameItem(object sender, RoutedEventArgs e)
        {

            if (RenameItemsContainer.SelectedIndex < 0) return;

            var selected = (RenameItemControl)RenameItemsContainer.SelectedItem;

            Settings.RenameFiles.Remove(selected.Item);
            RenameItems.Remove(selected);
        }

        private void SaveBuildsInfo(object sender, RoutedEventArgs e)
        {
            ((App)App.Current).Save();
        }
    }
}
