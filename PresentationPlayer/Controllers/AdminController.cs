using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BussinessLayer.Interfaces;
using BussinessLayer.DTOs;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly IPermissionService _permissionService;
        private readonly IUserService _userService;
        private readonly ISubjectService _subjectService;

        public AdminController(
            IPermissionService permissionService,
            IUserService userService,
            ISubjectService subjectService)
        {
            _permissionService = permissionService;
            _userService = userService;
            _subjectService = subjectService;
        }

        // GET: /Admin
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var permissions = await _permissionService.GetAllPermissionsAsync();
            return View(permissions);
        }

        // GET: /Admin/Grant
        [HttpGet]
        public async Task<IActionResult> Grant()
        {
            var lecturers = await _userService.GetAllLecturersAsync();
            var subjects = await _subjectService.GetAllAsync();
            ViewBag.Lecturers = lecturers;
            ViewBag.Subjects = subjects;
            return View(new GrantPermissionDto());
        }

        // POST: /Admin/Grant
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Grant(GrantPermissionDto model)
        {
            if (!ModelState.IsValid)
            {
                var lecturers = await _userService.GetAllLecturersAsync();
                var subjects = await _subjectService.GetAllAsync();
                ViewBag.Lecturers = lecturers;
                ViewBag.Subjects = subjects;
                return View(model);
            }

            // Check if permission already exists for this exact pair
            var exists = await _permissionService.PermissionExistsAsync(model.LecturerId, model.SubjectId);
            if (exists)
            {
                TempData["ErrorMessage"] = "Quyền này đã được cấp trước đó cho giảng viên này ở môn học này.";
                var lecturers = await _userService.GetAllLecturersAsync();
                var subjects = await _subjectService.GetAllAsync();
                ViewBag.Lecturers = lecturers;
                ViewBag.Subjects = subjects;
                return View(model);
            }

            // Check if the subject is already assigned to someone else
            if (model.CanUpload)
            {
                var isAssignedToOther = await _permissionService.IsSubjectAssignedToAnotherLecturerAsync(model.SubjectId, model.LecturerId);
                if (isAssignedToOther)
                {
                    var assignedName = await _permissionService.GetAssignedLecturerNameAsync(model.SubjectId);
                    TempData["ErrorMessage"] = $"Môn học này đã được cấp quyền upload cho giảng viên {assignedName ?? "khác"}. Mỗi môn học chỉ được 1 người upload.";
                    var lecturers = await _userService.GetAllLecturersAsync();
                    var subjects = await _subjectService.GetAllAsync();
                    ViewBag.Lecturers = lecturers;
                    ViewBag.Subjects = subjects;
                    return View(model);
                }
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var adminId))
                return Unauthorized();

            await _permissionService.GrantPermissionAsync(model, adminId);
            TempData["SuccessMessage"] = "Cấp quyền thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Revoke/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Revoke(long id)
        {
            await _permissionService.RevokePermissionAsync(id);
            TempData["SuccessMessage"] = "Đã thu hồi quyền thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
