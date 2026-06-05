using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using DataAccessPlayer.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System;
using System.Threading;
using BussinessLayer.Interfaces;

namespace BussinessLayer.Services
{
    public class LLMService : ILLMService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        // gemini-2.5-flash: model thế hệ mới nhất của Google Gemini
        private const string ModelName = "gemini-2.5-flash";
        private const int MaxHistoryTurns = 10;
        private const int MaxRetries = 3;

        public LLMService(IConfiguration config)
        {
            _apiKey = config["Gemini:ApiKey"]
                ?? throw new InvalidOperationException("Thiếu Gemini:ApiKey trong appsettings.json");
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(120);
        }

        public async Task<(string answer, IEnumerable<(Document doc, DocumentChunk chunk, float score)> citations)>
            GenerateAnswerAsync(string question, IEnumerable<RetrievalResult> contexts,
                                IEnumerable<ChatMessage>? history = null)
        {
            var contextList = contexts.ToList();
            var historyList = (history ?? Enumerable.Empty<ChatMessage>()).ToList();

            // ── System prompt ───────────────────────────────────────────────────
            var systemPrompt =
                "Bạn là trợ lý AI thông minh, thân thiện và chuyên nghiệp, " +
                "chuyên hỗ trợ người dùng tìm hiểu kiến thức từ tài liệu học tập.\n\n" +
                "NGUYÊN TẮC:\n" +
                "1. Ưu tiên trả lời dựa trên các đoạn tài liệu được cung cấp.\n" +
                "2. Nếu câu hỏi là lời chào, hỏi thăm hoặc hội thoại thông thường, hãy trả lời tự nhiên và thân thiện.\n" +
                "3. Nếu tài liệu không đủ thông tin, thông báo rõ ràng.\n" +
                "4. Nhớ bối cảnh hội thoại trước để trả lời mạch lạc.\n" +
                "5. Trả lời bằng ngôn ngữ của câu hỏi (tiếng Việt hoặc tiếng Anh).";

            // ── Context tài liệu ────────────────────────────────────────────────
            string docContext = "";
            if (contextList.Any())
            {
                docContext = "\n\n📚 TÀI LIỆU THAM KHẢO:\n" +
                    string.Join("\n---\n", contextList.Select((c, i) =>
                    {
                        var subjectName = c.Document.Subject != null ? c.Document.Subject.Name : "Không rõ môn học";
                        var docTitle = c.Document.Title ?? "Không rõ tài liệu";
                        return $"[Đoạn {i + 1}] (Môn học: {subjectName}, Tài liệu: {docTitle}): {c.Chunk.Content}";
                    }));
            }

            // ── Lịch sử hội thoại ──────────────────────────────────────────────
            var contents = new List<object>();
            foreach (var msg in historyList.TakeLast(MaxHistoryTurns * 2))
            {
                if (msg.Role != "user" && msg.Role != "assistant") continue;
                contents.Add(new
                {
                    role = msg.Role == "assistant" ? "model" : "user",
                    parts = new[] { new { text = msg.Content } }
                });
            }

            var currentText = string.IsNullOrEmpty(docContext)
                ? question
                : $"{docContext}\n\n❓ CÂU HỎI: {question}";

            contents.Add(new
            {
                role = "user",
                parts = new[] { new { text = currentText } }
            });

            var payload = new
            {
                systemInstruction = new { parts = new[] { new { text = systemPrompt } } },
                contents,
                generationConfig = new { temperature = 0.7, maxOutputTokens = 2048 }
            };

            var body = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{ModelName}:generateContent?key={_apiKey}";

            // ── Retry với backoff khi gặp timeout/429 ──────────────────────────
            for (int attempt = 1; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    // Tạo lại content mỗi lần vì HttpContent chỉ đọc được 1 lần
                    var reqContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                    var httpRes = await _httpClient.PostAsync(url, reqContent);

                    if (httpRes.IsSuccessStatusCode)
                    {
                        var json = await httpRes.Content.ReadAsStringAsync();
                        var doc = JsonDocument.Parse(json);
                        var answer = doc.RootElement
                            .GetProperty("candidates")[0]
                            .GetProperty("content")
                            .GetProperty("parts")[0]
                            .GetProperty("text")
                            .GetString();

                        var citations = contextList.Select(c => (c.Document, c.Chunk, c.Score));
                        return (answer ?? "", citations);
                    }

                    var statusCode = (int)httpRes.StatusCode;

                    if ((statusCode == 429 || statusCode == 503) && attempt < MaxRetries)
                    {
                        // Exponential backoff: 3s, 6s, 12s
                        var delay = (int)Math.Pow(2, attempt) * 1500;
                        await Task.Delay(delay);
                        continue;
                    }

                    // Lỗi không retry được
                    string friendlyMsg = statusCode switch
                    {
                        429 => "⏳ Hệ thống đang quá tải. Vui lòng đợi vài giây rồi thử lại.",
                        503 => "🔧 Dịch vụ AI tạm thời không khả dụng. Vui lòng thử lại sau.",
                        400 => "❌ Yêu cầu không hợp lệ. Vui lòng thử lại với câu hỏi khác.",
                        _   => $"❌ Lỗi từ AI ({statusCode}). Vui lòng thử lại."
                    };

                    return (friendlyMsg, Enumerable.Empty<(Document, DocumentChunk, float)>());
                }
                catch (TaskCanceledException)
                {
                    if (attempt < MaxRetries)
                    {
                        var delay = (int)Math.Pow(2, attempt) * 1500;
                        await Task.Delay(delay);
                        continue;
                    }
                    return ("⏱️ Yêu cầu quá lâu. Vui lòng thử lại.", Enumerable.Empty<(Document, DocumentChunk, float)>());
                }
                catch (Exception ex)
                {
                    return ($"❌ Lỗi: {ex.Message}", Enumerable.Empty<(Document, DocumentChunk, float)>());
                }
            }

            return ("❌ Không thể kết nối AI sau nhiều lần thử. Vui lòng thử lại sau.",
                    Enumerable.Empty<(Document, DocumentChunk, float)>());
        }
    }
}
