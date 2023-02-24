using System;
using System.Threading.Tasks;

namespace MediaVC
{
    public sealed class Repository : IRepository
    {
        #region Constructors

        public Repository(IRepositorySource source)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
        }

        internal Repository() { }

        #endregion

        #region Fields

        private IRepositorySource? source;

        #endregion

        #region Methods

        public ValueTask DownloadAsync()
        {
            throw new NotImplementedException();
        }

        public ValueTask UploadAsync()
        {
            throw new NotImplementedException();
        }

        public ValueTask AddChangeset(object changeset)
        {
            throw new NotImplementedException();
        }

        public static Repository Init()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
