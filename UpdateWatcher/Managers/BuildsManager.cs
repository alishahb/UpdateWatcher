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
        public delegate void DataInfo(string folder);
        public delegate void BuildInfo(IBuildData folder);
        public delegate void BuildsInfo(IList<IBuildData> folder);

        public event DataInfo OnBuildsDirectoryNotFound;
        public event BuildInfo OnBuildAdded;
        public event BuildsInfo OnBuildsAdded;


        public string BuildsFolder { get; }
        public ObservableCollection<IBuildData> Builds { get; }

        public BuildsManager(string buildsFolder, ObservableCollection<IBuildData> builds)
        {
            BuildsFolder = buildsFolder;
            Builds = builds;
        }

        public void UpdateInfo()
        {
            if (!Directory.Exists(BuildsFolder))
            {
                OnBuildsDirectoryNotFound?.Invoke(BuildsFolder);
                return;
            }

            var count = 0;
            List<IBuildData> addedBuilds = new List<IBuildData>();
            foreach (var file in Directory.GetFiles(BuildsFolder))
            {
                if (Builds.All(b => b.FullPath != file))
                {
                    var fileInfo = new FileInfo(file);

                    var build = new BuildData()
                    {
                        FileName = fileInfo.Name,
                        FileSize = fileInfo.Length,
                        FullPath = fileInfo.FullName
                    };
                    count++;
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
