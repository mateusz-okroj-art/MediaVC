using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MediaVC.Sources
{
    public sealed class DiskRepository : IRepositorySource, IDisposable, IAsyncDisposable
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



        protected void LoadFiles()
        {

        }

        protected async ValueTask ReleaseFiles()
        {
            foreach(var file in files)
                await file.DisposeAsync();

            files.Clear();
        }

        public static async Task<IActionResult<DiskRepository>> Init(string path)
        {
            if(Directory.Exists(path))
            {

            }

            throw new NotImplementedException();
        }

        public async void Dispose() => await ReleaseFiles();

        public ValueTask DisposeAsync() => ReleaseFiles();

        #endregion
    }
}
