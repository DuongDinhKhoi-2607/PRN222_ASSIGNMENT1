using System.Collections.Generic;
using System;
using BussinessLayer.Interfaces;

namespace BussinessLayer.Services
{
    public class ChunkingService : IChunkingService
    {
        public IEnumerable<ChunkDto> Chunk(string text, int maxSize = 1000)
        {
            if (text == null) yield break;
            int idx = 0;
            for (int i = 0; i < text.Length; i += maxSize)
            {
                var chunk = text.Substring(i, Math.Min(maxSize, text.Length - i));
                yield return new ChunkDto
                {
                    Index = idx++,
                    Text = chunk,
                    TokenCount = chunk.Length
                };
            }
        }
    }
}
