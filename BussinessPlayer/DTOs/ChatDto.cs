using System;

namespace BussinessLayer.DTOs
{
    public class ChatSessionDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long? SubjectId { get; set; }
        public string? Title { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ChatMessageDto
    {
        public long Id { get; set; }
        public long SessionId { get; set; }
        public string Role { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
    }

    public class MessageCitationDto
    {
        public long MessageId { get; set; }
        public long ChunkId { get; set; }
        public long DocumentId { get; set; }
        public double? RelevanceScore { get; set; }
        public string? Snippet { get; set; }
    }
}
