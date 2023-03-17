using System.Threading.Tasks;

namespace MediaVC
{
    /// <summary>
    /// Provides access to operate on single MediaVC repository
    /// </summary>
    public interface IRepository
    {
        Task<IActionResult> AddChangeset(object changeset);

        Task<IActionResult> DownloadAsync();

        Task<IActionResult> UploadAsync();
    }
}
