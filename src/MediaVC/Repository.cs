using System;
using System.Threading.Tasks;

namespace MediaVC
{
    public sealed class Repository : IRepository
    {
        #region Constructors

        public Repository(IRepositorySource source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }

        internal Repository() { }

        #endregion

        #region Fields

        #endregion

        #region Properties

        /// <summary>
        /// Repository realistic source
        /// </summary>
        public IRepositorySource? Source { get; private set; }

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

        #endregion
    }
}
