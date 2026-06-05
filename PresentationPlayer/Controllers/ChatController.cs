using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BussinessLayer.Interfaces;
using BussinessLayer.DTOs;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "student,lecturer")]
    public class ChatController : Controller
    {
        private readonly IChatService _chat;
        private readonly IRetrievalService _retrieval;
        private readonly ILLMService _llm;
        private readonly IMessageCitationService _citationSvc;
        private readonly ISubjectService _subjectService;

        public ChatController(
            IChatService chat,
            IRetrievalService retrieval,
            ILLMService llm,
            IMessageCitationService citationSvc,
            ISubjectService subjectService)
        {
            _chat = chat;
            _retrieval = retrieval;
            _llm = llm;
            _citationSvc = citationSvc;
            _subjectService = subjectService;
        }

        // GET: /Chat
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            long currentUserId = 0;
            if (userIdClaim != null && long.TryParse(userIdClaim.Value, out var id))
                currentUserId = id;

            var subjects = await _subjectService.GetAllAsync();
            ViewBag.Subjects = subjects;
            ViewBag.CurrentUserId = currentUserId;

            if (currentUserId > 0)
            {
                ViewBag.History = await _chat.GetSessionsAsync(currentUserId);
            }
            else
            {
                ViewBag.History = Enumerable.Empty<ChatSessionDto>();
            }

            return View("Session");
        }

        // GET: /Chat/Session/5
        [HttpGet]
        public async Task<IActionResult> Session(long id)
        {
            var session = await _chat.GetSessionAsync(id);
            if (session == null) return NotFound();

            var messages = await _chat.GetMessagesAsync(id);
            ViewBag.Session = session;
            ViewBag.Messages = messages;
            
            ViewBag.History = await _chat.GetSessionsAsync(session.UserId);

            return View();
        }

        // POST: /Chat/Send
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(long sessionId, string question)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            if (sessionId <= 0)
            {
                var title = question.Length > 30 ? question.Substring(0, 30) + "..." : question;
                var newSession = await _chat.CreateSessionAsync(userId, null, title);
                sessionId = newSession.Id;
            }
            else
            {
                var session = await _chat.GetSessionAsync(sessionId);
                if (session != null && string.IsNullOrWhiteSpace(session.Title))
                {
                    var newTitle = question.Length > 30 ? question.Substring(0, 30) + "..." : question;
                    await _chat.UpdateSessionTitleAsync(sessionId, newTitle);
                }
            }

            await _chat.AddMessageAsync(sessionId, "user", question);

            var contexts = await _retrieval.RetrieveAsync(question, null);
            // Pass history as null - ILLMService expects domain ChatMessage (not DTO)
            var (answer, citations) = await _llm.GenerateAnswerAsync(question, contexts, null);

            var assistantMsg = await _chat.AddMessageAsync(sessionId, "assistant", answer);

            foreach (var c in citations)
            {
                await _citationSvc.AddAsync(new MessageCitationDto
                {
                    MessageId = assistantMsg.Id,
                    ChunkId = c.chunk.Id,
                    DocumentId = c.doc.Id,
                    RelevanceScore = c.score,
                    Snippet = c.chunk.Content
                });
            }

            return RedirectToAction(nameof(Session), new { id = sessionId });
        }
    }
}
