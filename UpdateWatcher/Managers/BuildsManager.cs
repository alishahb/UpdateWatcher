using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Alisha.UpdateWatcher.Interfaces;
using Alisha.UpdateWatcher.Models;

namespace Alisha.UpdateWatcher.Managers
{
    class BuildsManager
    {
        public enum RemoveReason
        {
            FileNotFound,
            BuildsDirectoryChanged,
        }

        public delegate void DataInfo(string folder);

        public delegate void BuildInfo(IBuildData folder);
        public delegate void RemovedBuildInfo(IBuildData folder, RemoveReason reason);

        public delegate void BuildsInfo(IList<IBuildData> folder);



        public event DataInfo OnBuildsDirectoryNotFound;

        public event BuildInfo OnBuildAdded;
        public event RemovedBuildInfo OnBuildRemoved;

        public event BuildsInfo OnBuildsRemoved;
        public event BuildsInfo OnBuildsAdded;


        public ObservableCollection<IBuildData> Builds { get; }

        public BuildsManager(ObservableCollection<IBuildData> builds)
        {

            Builds = builds;
        }

        public void UpdateInfo(string buildsFolder)
        {
            var directoryInfo = new DirectoryInfo(buildsFolder);

            if (!Directory.Exists(buildsFolder))
            {
                OnBuildsDirectoryNotFound?.Invoke(buildsFolder);
                return;
            }


            var builds = Builds.ToList();
            foreach (var build in builds)
            {
                var info = new FileInfo(build.FullPath);

                if (!info.Exists)
                {
                    OnBuildRemoved?.Invoke(build, RemoveReason.FileNotFound);
                    Builds.Remove(build);
                    continue;
                }
                if (info.Directory.FullName != directoryInfo.FullName)
                {
                    OnBuildRemoved?.Invoke(build, RemoveReason.BuildsDirectoryChanged);
                    Builds.Remove(build);
                    continue;
                }
            }

            OnBuildsRemoved?.Invoke(builds.Except(Builds).ToList());

            List<IBuildData> addedBuilds = new List<IBuildData>();
            foreach (var file in Directory.GetFiles(buildsFolder))
            {
                var fileInfo = new FileInfo(file);

                if (Builds.All(b => b.FileName != fileInfo.Name))
                {
                    var build = new BuildData()
                    {
                        FileName = fileInfo.Name,
                        FileSize = fileInfo.Length,
                        FullPath = fileInfo.FullName
                    };

                    addedBuilds.Add(build);
                    Builds.Add(build);
                    OnBuildAdded?.Invoke(build);
                }
            }
            OnBuildsAdded?.Invoke(addedBuilds);


        }

        public bool IsIgnored(string fullPath) => Builds.Any(b => b.Ignore && b.FullPath == fullPath);
    }
}
