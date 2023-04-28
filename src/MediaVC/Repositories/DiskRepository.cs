using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MediaVC.Synchronizations;

namespace MediaVC.Repositories
{
    public sealed class DiskRepository : IRepository, IDisposable, IAsyncDisposable
    {
        #region Constructor

        public DiskRepository(string path)
        {
            if(string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if(!Directory.Exists(path))
                throw new DirectoryNotFoundException(nameof(path));

            var dir = new DirectoryInfo(path);

            if(dir is null)
                throw new DirectoryNotFoundException(nameof(path));

            if(!path.EndsWith(VersionControlDirectory))
            {
                workingDirectory = dir;

                dir = new DirectoryInfo(path)?.EnumerateDirectories()
                        ?.SingleOrDefault(dir => dir.Name == VersionControlDirectory);

                if(dir is null)
                    throw new DirectoryNotFoundException("Cannot found .vc directory.");
            }
            else
            {
                workingDirectory = dir?.Parent ?? throw new DirectoryNotFoundException("Cannot found working directory.");
            }

            var vc_files = dir.EnumerateFiles();
            var working_files = workingDirectory.EnumerateFiles("*", SearchOption.AllDirectories);

            if(!vc_files.Any(file => file.Name == "index.json"))
                throw new FileNotFoundException("Not found index.json.");

            TrackingFiles = new TrackingFilesList(new StreamWriter(trackingListFile));

            Synchronization = new DiskRepositorySynchronizer(dir);
        }

        #endregion

        #region Fields

        private readonly FileStream indexFile;

        private readonly FileStream trackingListFile;

        private readonly List<FileStream> files = new();

        private readonly DirectoryInfo workingDirectory;

        public readonly string VersionControlDirectory = ".vc";

        #endregion

        #region Properties

        public IRepositorySynchronization Synchronization { get; private set; }

        public IList<string> TrackingFiles { get; private set; }

        #endregion

        #region Methods

        public Task<IActionResult> AddChangeset(object changeset) => throw new NotImplementedException();

        public Task<IActionResult> DownloadAsync() => throw new NotImplementedException();

        public Task<IActionResult> UploadAsync() => throw new NotImplementedException();

        public Task<IActionResult> FetchInformation() => throw new NotImplementedException();

        public Task<IActionResult<IStatusInfo>> GetStatus() => throw new NotImplementedException();

        protected void LoadFiles()
        {

        }

        public static async Task<IActionResult<DiskRepository>> Init(string path)
        {
            if(Directory.Exists(path))
            {

            }

            throw new NotImplementedException();
        }

        protected async ValueTask ReleaseFiles()
        {
            foreach(var file in files)
                await file.DisposeAsync();

            files.Clear();
        }

        public async void Dispose() => await ReleaseFiles();

        public ValueTask DisposeAsync() => ReleaseFiles();

        #endregion
    }
}
