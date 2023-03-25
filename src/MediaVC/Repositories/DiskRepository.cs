using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using MediaVC.Synchronizations;

namespace MediaVC
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

            if(path.EndsWith(VersionControlDirectory))
            {
                var parent = Directory.GetParent(path);

                var vc = parent?.EnumerateDirectories()?.SingleOrDefault(dir => dir.Name == VersionControlDirectory);

                if(vc is null)
                    throw new DirectoryNotFoundException(nameof(path));

                //vc.EnumerateFiles();
            }

            Synchronization = new DiskRepositorySynchronizer();
        }

        #endregion

        #region Fields

        private readonly List<FileStream> files = new();

        public readonly string VersionControlDirectory = ".vc";

        #endregion

        #region Properties

        public IRepositorySynchronization Synchronization { get; private set; }

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
