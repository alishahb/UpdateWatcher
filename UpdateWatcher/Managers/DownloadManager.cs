using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using Alisha.UpdateWatcher.Annotations;

namespace Alisha.UpdateWatcher.Managers
{
    public enum DownloadResult
    {
        Idle = 0,
        Started = 1,
        NotChanged = 2,
        Failed = 3,
        Success = 4,
    }

    public delegate void FileInfoHandler(FileInfo info);


    public class DownloadManager : INotifyPropertyChanged
    {

        public event FileInfoHandler OnDownloadDataCompleted;
        public event DownloadProgressChangedEventHandler OnDownloadProgressChanged;
        public event FileInfoHandler OnDownloadedFileSaved;
        public event FileInfoHandler OnDownloadStarted;


        private static object locker = new Object();

        public string Path { get; set; }
        public AsyncCompletedEventArgs Result { get; protected set; }
        public string ErrorMessage { get; protected set; }


        #region DownloadResult
        protected DownloadResult _downloadResult;
        private string _destination;

        public DownloadResult DownloadResult
        {
            get { return _downloadResult; }
            set {
                _downloadResult = value; OnPropertyChanged();
                DownloadResultString = value.ToString();
            }
        }

        #region DownloadResultString
        protected string _downloadResultString;
        public string DownloadResultString
        {
            get { return _downloadResultString; }
            set { _downloadResultString = value; OnPropertyChanged(); }
        }
        #endregion EndOf-DownloadResultString


        #endregion EndOf-DownloadResult

        #region FileName
        protected string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; OnPropertyChanged(); }
        }
        #endregion EndOf-FileName




        public DownloadManager(string path)
        {
            Path = path;
            DownloadResult = DownloadResult.Idle;

        }

        public void Execute(string url)
        {
            DownloadResult = DownloadResult.Idle;

            if (string.IsNullOrEmpty(url))
            {
                SetDownloadResult(DownloadResult.Failed, null);
                return;
            }

            using (var client = new WebClient())
            {
                //WebRequest request = WebRequest.Create(Url);
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.KeepAlive = false;
                request.Timeout = 5000;
                request.Proxy = null;

                request.ServicePoint.ConnectionLeaseTimeout = 5000;
                request.ServicePoint.MaxIdleTime = 5000;

                WebResponse response;

                try
                {
                    response = request.GetResponse();

                }
                catch (Exception e)
                {
                    ErrorMessage = e.Message;
                    SetDownloadResult(DownloadResult.Failed, null);
                    return;
                }

                FileName = System.IO.Path.GetFileName(response.ResponseUri.LocalPath) ?? Guid.NewGuid().ToString();


                OnDownloadStarted?.Invoke(new FileInfo(FileName));

                if (response.Headers["Content-Disposition"] != null)
                    FileName = new ContentDisposition(response.Headers["Content-Disposition"])?.FileName;

                if (!Directory.Exists(Path))
                    Directory.CreateDirectory(Path);


                _destination = System.IO.Path.Combine(Path, FileName);


                var info = new FileInfo(_destination);

                if (info.Exists)
                {
                    if (info.Length == response.ContentLength && FileName == info.Name)
                    {
                        SetDownloadResult(DownloadResult.NotChanged, info);
                        return;
                    }
                }




                if (response.ContentLength == 0)
                {
                    SetDownloadResult(DownloadResult.Failed, info);
                    return;
                }

                DownloadResult = DownloadResult.Started;

                response.Dispose();

                client.DownloadDataCompleted += DownloadDataCompleted;
                client.DownloadProgressChanged += DownloadProgressChanged;
                client.DownloadDataAsync(new Uri(url));

            }
        }
        private readonly Stopwatch _delay = new Stopwatch();
        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            OnDownloadProgressChanged?.Invoke(sender, e);
        }

        private void DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)

        {
            var info = new FileInfo(_destination);
            SetDownloadResult(DownloadResult.Success, info);

            var timer = new Stopwatch();
            timer.Start();
            while (!new FileInfo(_destination).Exists)
            {
                Thread.Sleep(50);
                Thread.Yield();
                if (timer.ElapsedMilliseconds > 5000)
                    break;
            }

            var data = e.Result;


            lock (locker)
            {
                using (var writer = new FileStream(_destination, FileMode.Create, FileAccess.Write))
                {
                    writer.Write(data, 0, data.Length);
                    writer.Flush();
                    writer.Close();
                }
            }

            info = new FileInfo(_destination);
            OnDownloadedFileSaved?.Invoke(info);

        }

        private FileInfo SetDownloadResult(DownloadResult downloadResult, FileInfo info)
        {
            DownloadResult = downloadResult;

            OnDownloadDataCompleted?.Invoke(info);

            if(DownloadResult != DownloadResult.Success)
                OnDownloadedFileSaved?.Invoke(info);

            return info;
        }



        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Application.Current.Dispatcher.Invoke(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
        }
    }
}
