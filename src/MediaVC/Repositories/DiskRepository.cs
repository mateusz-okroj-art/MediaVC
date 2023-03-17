using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MediaVC.Sources
{
    public sealed class DiskRepository : IRepository, IDisposable, IAsyncDisposable
    {
        #region Constructor

        public DiskRepository(string path)
        {
            
        }

        #endregion

        #region Fields

        private List<FileStream> files;

        #endregion

        #region Methods

        public Task<IActionResult> AddChangeset(object changeset) => throw new NotImplementedException();

        public Task<IActionResult> DownloadAsync() => throw new NotImplementedException();

        public Task<IActionResult> UploadAsync() => throw new NotImplementedException();

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
