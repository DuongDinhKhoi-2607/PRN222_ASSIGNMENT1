using System.Threading.Tasks;

namespace BussinessLayer.Interfaces
{
    public interface IChapterService
    {
        Task<bool> ExistsAsync(long chapterId);
    }
}
