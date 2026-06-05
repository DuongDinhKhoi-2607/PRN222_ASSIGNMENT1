using System.Collections.Generic;
using System.Threading.Tasks;
using BussinessLayer.Services;

namespace BussinessLayer.Interfaces
{
    public interface IRetrievalService
    {
        /// <param name="question">Câu hỏi của người dùng.</param>
        /// <param name="subjectId">Lọc theo môn học. null = tìm toàn bộ tài liệu.</param>
        /// <param name="topK">Số chunk tối đa trả về (sau khi đã lọc ngưỡng).</param>
        /// <param name="minScore">Ngưỡng tối thiểu cosine similarity (0–1). Mặc định 0.4.</param>
        Task<IEnumerable<RetrievalResult>> RetrieveAsync(
            string question,
            long? subjectId = null,
            int topK = 15,
            float minScore = 0.1f);
    }
}
