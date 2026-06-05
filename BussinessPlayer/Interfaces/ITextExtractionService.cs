using System.Threading.Tasks;

namespace BussinessLayer.Interfaces
{
    public interface ITextExtractionService
    {
        Task<string> ExtractTextAsync(string filePath);
    }
}
