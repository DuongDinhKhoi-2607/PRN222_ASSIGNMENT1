using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BussinessLayer.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveAsync(IFormFile file);
    }
}
