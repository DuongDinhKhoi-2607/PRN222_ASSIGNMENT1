using DataAccessLayer.Repositories;
using DataAccessPlayer.Models;
using System.Threading.Tasks;
using BussinessLayer.Interfaces;
using BussinessLayer.DTOs;

namespace BussinessLayer.Services
{
    public class CitationService : IMessageCitationService
    {
        private readonly MessageCitationRepository _repo;
        public CitationService(MessageCitationRepository repo) { _repo = repo; }

        public async Task<MessageCitationDto> AddAsync(MessageCitationDto dto)
        {
            var ent = new MessageCitation
            {
                MessageId = dto.MessageId,
                ChunkId = dto.ChunkId,
                DocumentId = dto.DocumentId,
                RelevanceScore = dto.RelevanceScore,
                Snippet = dto.Snippet
            };
            var created = await _repo.AddAsync(ent);
            return new MessageCitationDto
            {
                MessageId = created.MessageId,
                ChunkId = created.ChunkId,
                DocumentId = created.DocumentId,
                RelevanceScore = created.RelevanceScore,
                Snippet = created.Snippet
            };
        }
    }
}
