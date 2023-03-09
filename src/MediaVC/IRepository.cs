using System.Threading.Tasks;

namespace MediaVC
{
    /// <summary>
    /// Provides access to operate on single MediaVC repository
    /// </summary>
    public interface IRepository
    {
        ValueTask AddChangeset(object changeset);

        ValueTask DownloadAsync();

        ValueTask UploadAsync();
    }
}
