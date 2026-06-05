using DataAccessLayer.Repositories;
using DataAccessPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using UglyToad.PdfPig;
using Microsoft.AspNetCore.Http;
using BussinessLayer.Interfaces;

namespace BussinessLayer.Services
{
    public class DocumentIngestionService : IDocumentIngestionService
    {
        private readonly DocumentRepository _docRepo;
        private readonly DocumentChunkRepository _chunkRepo;
        private readonly ChunkEmbeddingRepository _embeddingRepo;
        private readonly IFileStorageService _storage;
        private readonly ITextExtractionService _extractor;
        private readonly IChunkingService _chunker;
        private readonly IEmbeddingService _embedder;

        public DocumentIngestionService(DocumentRepository docRepo, DocumentChunkRepository chunkRepo, ChunkEmbeddingRepository embeddingRepo, IFileStorageService storage, ITextExtractionService extractor, IChunkingService chunker, IEmbeddingService embedder)
        {
            _docRepo = docRepo;
            _chunkRepo = chunkRepo;
            _embeddingRepo = embeddingRepo;
            _storage = storage;
            _extractor = extractor;
            _chunker = chunker;
            _embedder = embedder;
        }

        public async Task<Document> IngestAsync(IFormFile file, string title, long subjectId, long? chapterId = null, long? userId = null)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            // Save file to disk using storage service
            var filePath = await _storage.SaveAsync(file);

            // Extract text
            var text = await _extractor.ExtractTextAsync(filePath);

            // Create document record
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var doc = new Document
            {
                Title = string.IsNullOrWhiteSpace(title) ? file.FileName : title,
                FileName = file.FileName,
                FilePath = filePath,
                FileType = ext.TrimStart('.'),
                FileSize = file.Length,
                SubjectId = subjectId,
                ChapterId = chapterId,
                UserId = userId,
                UploadedBy = userId,
                Status = "uploaded",
                UploadedAt = DateTime.UtcNow
            };

            await _docRepo.AddAsync(doc);

            // Chunk and embed
            var chunks = _chunker.Chunk(text, 1000);
            foreach (var dto in chunks)
            {
                var chunk = new DocumentChunk
                {
                    DocumentId = doc.Id,
                    ChunkIndex = dto.Index,
                    Content = dto.Text,
                    TokenCount = dto.TokenCount,
                    PageNumber = 0,
                    CreatedAt = DateTime.UtcNow,
                    ChunkingStrategyId = 1
                };
                await _chunkRepo.AddAsync(chunk);

                var vector = await _embedder.EmbedAsync(dto.Text);
                var embedding = new ChunkEmbedding
                {
                    ChunkId = chunk.Id,
                    EmbeddingModelId = 1,
                    Vector = string.Join(',', vector.Select(v => v.ToString(System.Globalization.CultureInfo.InvariantCulture))),
                    Dimension = vector.Length,
                    CreatedAt = DateTime.UtcNow
                };
                await _embeddingRepo.AddAsync(embedding);
            }

            doc.Status = "indexed";
            doc.IndexedAt = DateTime.UtcNow;
            await _docRepo.UpdateAsync(doc);

            return doc;
        }
    }
}