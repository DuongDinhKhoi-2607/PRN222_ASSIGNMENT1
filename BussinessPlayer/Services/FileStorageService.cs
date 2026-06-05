using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System;
using BussinessLayer.Interfaces;

namespace BussinessLayer.Services
{
    public class FileStorageService : IFileStorageService
    {
        public async Task<string> SaveAsync(IFormFile file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploads);
            var filePath = Path.Combine(uploads, Path.GetFileName(file.FileName));
            using (var stream = File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }
            return filePath;
        }
    }
}
