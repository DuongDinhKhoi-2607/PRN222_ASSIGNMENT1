using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System;
using BussinessLayer.Interfaces;

namespace BussinessLayer.Services
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private DateTime _lastRequestTime = DateTime.MinValue;
        private const int RequestDelayMs = 2000; // 2s delay giữa requests

        private const int GeminiDimension = 256;
        private const string ModelName = "gemini-embedding-2";
        private const int BatchSize = 2; // Giảm batch size

        public EmbeddingService(IConfiguration config)
        {
            _apiKey = config["Gemini:ApiKey"]
                ?? throw new InvalidOperationException("Thiếu Gemini:ApiKey trong appsettings.json");
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
        }

        private async Task ThrottleAsync()
        {
            var elapsed = DateTime.UtcNow - _lastRequestTime;
            if (elapsed.TotalMilliseconds < RequestDelayMs)
                await Task.Delay((int)(RequestDelayMs - elapsed.TotalMilliseconds));
            _lastRequestTime = DateTime.UtcNow;
        }

        public async Task<float[]> EmbedAsync(string text, int dimension = GeminiDimension)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text không được rỗng");

            await ThrottleAsync();

            var payload = new
            {
                model = $"models/{ModelName}",
                content = new { parts = new[] { new { text } } },
                outputDimensionality = dimension
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var res = await _httpClient.PostAsync(
                $"https://generativelanguage.googleapis.com/v1beta/models/{ModelName}:embedContent?key={_apiKey}",
                content);

            if (!res.IsSuccessStatusCode)
            {
                var errorContent = await res.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Embedding API lỗi {(int)res.StatusCode}: {errorContent}");
            }

            var json = await res.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            return doc.RootElement
                .GetProperty("embedding")
                .GetProperty("values")
                .EnumerateArray()
                .Select(x => x.GetSingle())
                .ToArray();
        }

        public async Task<float[][]> EmbedBatchAsync(IEnumerable<string> texts, int dimension = GeminiDimension)
        {
            var list = texts.ToList();
            if (list.Count == 0) return Array.Empty<float[]>();

            var results = new List<float[]>();

            for (int i = 0; i < list.Count; i += BatchSize)
            {
                var batch = list.Skip(i).Take(BatchSize).ToList();
                var batchResults = await ProcessBatchAsync(batch, dimension);
                results.AddRange(batchResults);
            }

            return results.ToArray();
        }

        private async Task<List<float[]>> ProcessBatchAsync(List<string> texts, int dimension)
        {
            await ThrottleAsync();

            var requests = texts.Select(t => new
            {
                model = $"models/{ModelName}",
                content = new { parts = new[] { new { text = t } } },
                outputDimensionality = dimension
            }).ToArray();

            var payload = new { requests };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var res = await _httpClient.PostAsync(
                $"https://generativelanguage.googleapis.com/v1beta/models/{ModelName}:batchEmbedContents?key={_apiKey}",
                content);

            if (!res.IsSuccessStatusCode)
            {
                var errorContent = await res.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Batch Embedding API lỗi {(int)res.StatusCode}: {errorContent}");
            }

            var json = await res.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            return doc.RootElement
                .GetProperty("embeddings")
                .EnumerateArray()
                .Select(e => e.GetProperty("values")
                    .EnumerateArray()
                    .Select(v => v.GetSingle())
                    .ToArray())
                .ToList();
        }
    }
}
