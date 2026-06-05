using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessPlayer.Models;
using BussinessLayer.Services;

namespace BussinessLayer.Interfaces
{
    public interface ILLMService
    {
        Task<(string answer, IEnumerable<(Document doc, DocumentChunk chunk, float score)> citations)>
            GenerateAnswerAsync(string question, IEnumerable<RetrievalResult> contexts,
                                IEnumerable<ChatMessage>? history = null);
    }
}
