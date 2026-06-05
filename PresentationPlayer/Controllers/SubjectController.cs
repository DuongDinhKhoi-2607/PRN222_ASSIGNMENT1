using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BussinessLayer.Interfaces;
using BussinessLayer.DTOs;
using System;
using System.Threading.Tasks;

namespace PresentationLayer.Controllers
{
    [Authorize]
    public class SubjectController : Controller
    {
        private readonly ISubjectService _svc;
        public SubjectController(ISubjectService svc) { _svc = svc; }

        // GET: /Subject
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var list = await _svc.GetAllAsync();
            return View(list);
        }

        // GET: /Subject/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(long id)
        {
            var subject = await _svc.GetByIdAsync(id);
            if (subject == null) return NotFound();
            return View(subject);
        }

        // GET: /Subject/Create
        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Subject/Create
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubjectDto subject)
        {
            if (!ModelState.IsValid) return View(subject);

            bool isUnique = await _svc.IsCodeUniqueAsync(subject.Code);
            if (!isUnique)
            {
                ModelState.AddModelError("Code", "Mã môn học này đã tồn tại.");
                return View(subject);
            }

            await _svc.CreateAsync(subject);
            TempData["SuccessMessage"] = "Tạo môn học thành công!";
            return RedirectToAction(nameof(Create));
        }

        // GET: /Subject/Edit/5
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var subject = await _svc.GetByIdAsync(id);
            if (subject == null) return NotFound();
            return View(subject);
        }

        // POST: /Subject/Edit/5
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, SubjectDto subject)
        {
            if (id != subject.Id) return BadRequest();
            if (!ModelState.IsValid) return View(subject);

            bool isUnique = await _svc.IsCodeUniqueAsync(subject.Code, id);
            if (!isUnique)
            {
                ModelState.AddModelError("Code", "Mã môn học này đã tồn tại.");
                return View(subject);
            }

            await _svc.UpdateAsync(subject);
            TempData["SuccessMessage"] = "Cập nhật môn học thành công!";
            return RedirectToAction(nameof(Edit), new { id = subject.Id });
        }

        // GET: /Subject/Delete/5
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var subject = await _svc.GetByIdAsync(id);
            if (subject == null) return NotFound();
            return View(subject);
        }

        // POST: /Subject/Delete/5
        [Authorize(Roles = "admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            try
            {
                await _svc.DeleteAsync(id);
                TempData["SuccessMessage"] = "Xóa môn học thành công!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Không thể xóa môn học này vì đã có dữ liệu (tài liệu, câu hỏi, phiên chat...) liên kết với nó.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
