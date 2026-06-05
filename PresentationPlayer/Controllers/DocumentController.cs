using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using BussinessLayer.Interfaces;
using BussinessLayer.DTOs;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace PresentationLayer.Controllers
{
    [Authorize]
    public class DocumentController : Controller
    {
        private readonly IDocumentIngestionService _ingest;
        private readonly ISubjectService _subjectService;
        private readonly IChapterService _chapterService;
        private readonly IDocumentService _docService;
        private readonly IPermissionService _permissionService;

        public DocumentController(
            IDocumentIngestionService ingest,
            ISubjectService subjectService,
            IChapterService chapterService,
            IDocumentService docService,
            IPermissionService permissionService)
        {
            _ingest = ingest;
            _subjectService = subjectService;
            _chapterService = chapterService;
            _docService = docService;
            _permissionService = permissionService;
        }

        // Redirect root to subject
        [AllowAnonymous]
        [HttpGet("/")]
        public IActionResult Root()
        {
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("student"))
                return RedirectToAction("Index", "Chat");
            return RedirectToAction("Index", "Subject");
        }

        // GET: /Document
        [Authorize(Roles = "lecturer,admin")]
        [HttpGet]
        public async Task<IActionResult> Index(long? subjectId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var docs = await _docService.GetAllAsync();

            if (subjectId.HasValue)
            {
                docs = docs.Where(d => d.SubjectId == subjectId.Value);
            }

            var subjects = await _subjectService.GetAllAsync();
            ViewBag.Subjects = subjects;
            ViewBag.CurrentSubjectId = subjectId;

            // Pass allowed subject IDs for permission-based UI rendering
            if (roleClaim == "admin")
            {
                ViewBag.AllowedSubjectIds = subjects.Select(s => s.Id).ToList();
                ViewBag.IsAdmin = true;
            }
            else
            {
                var allowedIds = await _permissionService.GetAllowedSubjectIdsAsync(userId);
                ViewBag.AllowedSubjectIds = allowedIds.ToList();
                ViewBag.IsAdmin = false;
            }

            return View(docs);
        }

        // GET: /Document/Details/5
        [Authorize(Roles = "lecturer,admin")]
        [HttpGet]
        public async Task<IActionResult> Details(long id, string returnUrl = null)
        {
            var doc = await _docService.GetByIdWithSubjectAsync(id);
            if (doc == null) return NotFound();
            
            ViewBag.ReturnUrl = returnUrl;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim != null ? long.Parse(userIdClaim.Value) : 0;
            
            if (roleClaim == "admin")
            {
                ViewBag.IsAdmin = true;
                ViewBag.AllowedSubjectIds = new List<long>();
            }
            else
            {
                ViewBag.IsAdmin = false;
                var allowedIds = await _permissionService.GetAllowedSubjectIdsAsync(userId);
                ViewBag.AllowedSubjectIds = allowedIds.ToList();
            }
            var chunks = await _docService.GetChunksByDocumentIdAsync(id);
            var sortedChunks = chunks.OrderBy(c => c.ChunkIndex).ToList();
            ViewBag.Chunks = sortedChunks;
            ViewBag.ChunksCount = sortedChunks.Count;

            return View(doc);
        }

        // GET: /Document/Upload
        [Authorize(Roles = "lecturer,admin")]
        [HttpGet]
        public async Task<IActionResult> Upload(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (roleClaim == "lecturer")
            {
                if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var lecturerId))
                    return Unauthorized();

                var allowedIds = await _permissionService.GetAllowedSubjectIdsAsync(lecturerId);
                var allowedList = allowedIds.ToList();

                if (!allowedList.Any())
                {
                    TempData["ErrorMessage"] = "Bạn chưa được cấp quyền upload cho môn học nào. Vui lòng liên hệ Admin.";
                    return RedirectToAction(nameof(Index));
                }

                var allSubjects = await _subjectService.GetAllAsync();
                ViewBag.Subjects = allSubjects.Where(s => allowedList.Contains(s.Id));
            }
            else
            {
                var subjects = await _subjectService.GetAllAsync();
                ViewBag.Subjects = subjects;
            }

            return View();
        }

        // POST: /Document/Upload
        [Authorize(Roles = "lecturer,admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file, string title, long subjectId, long? chapterId, string returnUrl)
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            // Permission check for lecturer
            if (roleClaim == "lecturer")
            {
                if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var lecturerId))
                    return Unauthorized();

                var hasPermission = await _permissionService.HasUploadPermissionAsync(lecturerId, subjectId);
                if (!hasPermission)
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền upload tài liệu cho môn học này.";
                    return RedirectToAction(nameof(Index));
                }
            }

            // Reload subjects for form
            if (roleClaim == "lecturer" && userIdClaim != null && long.TryParse(userIdClaim.Value, out var uid))
            {
                var allowedIds = await _permissionService.GetAllowedSubjectIdsAsync(uid);
                var allSubjects = await _subjectService.GetAllAsync();
                ViewBag.Subjects = allSubjects.Where(s => allowedIds.Contains(s.Id));
            }
            else
            {
                var subjects = await _subjectService.GetAllAsync();
                ViewBag.Subjects = subjects;
            }

            if (file == null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "Vui lòng chọn tệp để upload." });
                ModelState.AddModelError("", "Vui lòng chọn tệp để upload.");
                return View();
            }

            var subject = await _subjectService.GetByIdAsync(subjectId);
            if (subject == null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = $"Môn học Id={subjectId} không tồn tại." });
                ModelState.AddModelError("", $"Môn học Id={subjectId} không tồn tại.");
                return View();
            }

            if (chapterId.HasValue)
            {
                var chapterExists = await _chapterService.ExistsAsync(chapterId.Value);
                if (!chapterExists)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return Json(new { success = false, message = $"Chapter Id={chapterId.Value} không tồn tại." });
                    ModelState.AddModelError("", $"Chapter Id={chapterId.Value} không tồn tại.");
                    return View();
                }
            }

            // Check for duplicate document
            var existingDocs = await _docService.GetBySubjectIdAsync(subjectId);
            if (existingDocs.Any(d => d.Title.Equals(title, StringComparison.OrdinalIgnoreCase) || d.FileName.Equals(file.FileName, StringComparison.OrdinalIgnoreCase)))
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "Đã có tài liệu trùng tên file hoặc tiêu đề trong môn học này." });
                ModelState.AddModelError("", "Đã có tài liệu trùng tên file hoặc tiêu đề trong môn học này.");
                return View();
            }

            long? userId = null;
            if (userIdClaim != null && long.TryParse(userIdClaim.Value, out var parsedId))
                userId = parsedId;

            try
            {
                await _ingest.IngestAsync(file, title, subjectId, chapterId, userId);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Tài liệu đã được upload và xử lý thành công!" });
                }

                TempData["SuccessMessage"] = "Tài liệu đã được upload và xử lý thành công!";
                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return LocalRedirect(returnUrl);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Lỗi: " + (ex.InnerException?.Message ?? ex.Message) });
                }

                ModelState.AddModelError("", "Lỗi: " + (ex.InnerException?.Message ?? ex.Message));
                return View();
            }
        }

        // GET: /Document/Edit/5
        [Authorize(Roles = "lecturer,admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(long id, string returnUrl = null)
        {
            var doc = await _docService.GetByIdAsync(id);
            if (doc == null) return NotFound();

            ViewBag.ReturnUrl = returnUrl;

            // Permission check for lecturer
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            if (roleClaim == "lecturer")
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var lecturerId))
                    return Unauthorized();

                var hasPermission = await _permissionService.HasUploadPermissionAsync(lecturerId, doc.SubjectId);
                if (!hasPermission)
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền chỉnh sửa tài liệu cho môn học này.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var subjects = await _subjectService.GetAllAsync();
            ViewBag.Subjects = subjects;
            return View(doc);
        }

        // POST: /Document/Edit/5
        [Authorize(Roles = "lecturer,admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, string title, long subjectId, string returnUrl)
        {
            var doc = await _docService.GetByIdAsync(id);
            if (doc == null) return NotFound();

            // Permission check for lecturer
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            if (roleClaim == "lecturer")
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var lecturerId))
                    return Unauthorized();

                var hasPermission = await _permissionService.HasUploadPermissionAsync(lecturerId, doc.SubjectId);
                if (!hasPermission)
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền chỉnh sửa tài liệu cho môn học này.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var subject = await _subjectService.GetByIdAsync(subjectId);
            if (subject == null)
            {
                ModelState.AddModelError("", $"Môn học Id={subjectId} không tồn tại.");
                var subjects = await _subjectService.GetAllAsync();
                ViewBag.Subjects = subjects;
                return View(doc);
            }

            await _docService.UpdateAsync(id, title, subjectId);
            TempData["SuccessMessage"] = "Cập nhật tài liệu thành công!";
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Document/Delete/5
        [Authorize(Roles = "lecturer,admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(long id, string returnUrl = null)
        {
            var doc = await _docService.GetByIdAsync(id);
            if (doc == null) return NotFound();

            ViewBag.ReturnUrl = returnUrl;

            // Permission check for lecturer
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            if (roleClaim == "lecturer")
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var lecturerId))
                    return Unauthorized();

                var hasPermission = await _permissionService.HasUploadPermissionAsync(lecturerId, doc.SubjectId);
                if (!hasPermission)
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền xóa tài liệu cho môn học này.";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(doc);
        }

        // POST: /Document/Delete/5
        [Authorize(Roles = "lecturer,admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id, string returnUrl)
        {
            var doc = await _docService.GetByIdAsync(id);
            if (doc == null) return NotFound();

            // Permission check for lecturer
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            if (roleClaim == "lecturer")
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var lecturerId))
                    return Unauthorized();

                var hasPermission = await _permissionService.HasUploadPermissionAsync(lecturerId, doc.SubjectId);
                if (!hasPermission)
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền xóa tài liệu cho môn học này.";
                    return RedirectToAction(nameof(Index));
                }
            }

            await _docService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Xóa tài liệu thành công!";
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);
            return RedirectToAction(nameof(Index));
        }
    }
}
