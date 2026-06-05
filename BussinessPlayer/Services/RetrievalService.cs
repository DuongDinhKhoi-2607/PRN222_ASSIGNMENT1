using DataAccessLayer.Repositories;
using DataAccessPlayer.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using BussinessLayer.Interfaces;

namespace BussinessLayer.Services
{
    public class RetrievalResult
    {
        public Document Document { get; set; }
        public DocumentChunk Chunk { get; set; }
        public float Score { get; set; }
    }

    public class RetrievalService : IRetrievalService
    {
        private readonly ChunkEmbeddingRepository _embeddingRepo;
        private readonly IEmbeddingService _embedder;

        public RetrievalService(ChunkEmbeddingRepository embeddingRepo, IEmbeddingService embedder)
        {
            _embeddingRepo = embeddingRepo;
            _embedder = embedder;
        }

        public async Task<IEnumerable<RetrievalResult>> RetrieveAsync(
            string question,
            long? subjectId = null,
            int topK = 15,
            float minScore = 0.1f)
        {
            // Lấy embeddings: lọc theo môn nếu có subjectId
            var all = subjectId.HasValue
                ? (await _embeddingRepo.GetBySubjectAsync(subjectId.Value)).ToList()
                : (await _embeddingRepo.GetAllWithChunkAsync()).ToList();

            if (!all.Any()) return Enumerable.Empty<RetrievalResult>();

            // Nhóm các vector câu hỏi theo các chiều unique trong DB để tránh mismatch
            var uniqueDims = all.Select(e => e.Dimension).Distinct().ToList();
            var qvecs = new Dictionary<int, float[]>();
            foreach (var dim in uniqueDims)
            {
                qvecs[dim] = await _embedder.EmbedAsync(question, dim);
            }

            var scored = all
                .Select(e =>
                {
                    var vec = e.Vector
                        .Split(',')
                        .Select(s => float.Parse(s, System.Globalization.CultureInfo.InvariantCulture))
                        .ToArray();

                    var qvec = qvecs.TryGetValue(e.Dimension, out var qv) ? qv : null;
                    if (qvec == null || qvec.Length != vec.Length) return null;

                    return new RetrievalResult
                    {
                        Document = e.Chunk.Document,
                        Chunk    = e.Chunk,
                        Score    = CosineSimilarity(qvec, vec)
                    };
                })
                .Where(r => r != null && r.Score >= minScore)   // Chỉ lấy chunk thực sự liên quan
                .OrderByDescending(r => r!.Score)
                .Take(topK)
                .Select(r => r!)
                .ToList();

            return scored;
        }

        private static float CosineSimilarity(float[] a, float[] b)
        {
            if (a.Length != b.Length) return 0f;
            double dot = 0, na = 0, nb = 0;
            for (int i = 0; i < a.Length; i++)
            {
                dot += a[i] * b[i];
                na  += a[i] * a[i];
                nb  += b[i] * b[i];
            }
            return (float)(dot / (Math.Sqrt(na) * Math.Sqrt(nb) + 1e-8));
        }
    }
}
