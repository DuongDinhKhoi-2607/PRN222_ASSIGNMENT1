using DataAccessLayer.Repositories;
using DataAccessPlayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BussinessLayer.Interfaces;

namespace BussinessLayer.Services
{
    public class ChapterService : IChapterService
    {
        private readonly RagchatbotDbContext _db;

        public ChapterService(RagchatbotDbContext db)
        {
            _db = db;
        }

        public async Task<bool> ExistsAsync(long chapterId)
        {
            return await _db.Chapters.AnyAsync(c => c.Id == chapterId);
        }
    }
}
