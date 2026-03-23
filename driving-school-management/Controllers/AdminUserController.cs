using Microsoft.AspNetCore.Mvc;
using driving_school_management.ViewModels;

public class AdminUserController : Controller
{
    private readonly AdminUserService _adminuserService;

    public AdminUserController(AdminUserService userService)
    {
        _adminuserService = userService;
    }

    public IActionResult Index()
    {
        var data = _adminuserService.GetUsers();
        return View(data);
    }

    public IActionResult Detail(int id)
    {
        var data = _adminuserService.GetUserDetail(id);

        if (data == null)
            return NotFound();

        return View(data);
    }

    public IActionResult Edit(int id)
    {
        var currentUserId = HttpContext.Session.GetInt32("UserId");
        var currentRoleId = HttpContext.Session.GetInt32("RoleId");

        if (!currentUserId.HasValue)
            return RedirectToAction("Login", "Auth");

        if (currentRoleId != 1)
            return Forbid();

        var data = _adminuserService.GetUserDetail(id);

        if (data == null)
            return NotFound();

        var vm = new UserUpdateVM
        {
            UserId = Convert.ToInt32(data.userId),
            UserName = Convert.ToString(data.userName),
            HoTen = Convert.ToString(data.hoTen),
            Sdt = Convert.ToString(data.sdt),
            Email = Convert.ToString(data.email),
            IsActive = Convert.ToInt32(data.isActive)
        };

        ViewBag.IsSelfEdit = currentUserId.Value == vm.UserId;

        return View(vm);
    }

    [HttpPost]
    public IActionResult Edit(UserUpdateVM model)
    {
        var currentUserId = HttpContext.Session.GetInt32("UserId");
        var currentRoleId = HttpContext.Session.GetInt32("RoleId");

        if (!currentUserId.HasValue)
            return RedirectToAction("Login", "Auth");

        if (currentRoleId != 1)
            return Forbid();

        var oldData = _adminuserService.GetUserDetail(model.UserId);
        if (oldData == null)
            return NotFound();

        if (currentUserId.Value == model.UserId)
        {
            model.IsActive = Convert.ToInt32(oldData.isActive);
        }

        _adminuserService.UpdateUser(model);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult UpdateRole([FromBody] RoleUpdateVM model)
    {
        if (model == null)
            return Json(new { success = false, message = "Dữ liệu gửi lên không hợp lệ." });

        if (model.UserId <= 0 || model.RoleId <= 0)
            return Json(new { success = false, message = "Thiếu userId hoặc roleId." });

        if (string.IsNullOrWhiteSpace(model.AdminPassword))
            return Json(new { success = false, message = "Thiếu mật khẩu admin." });

        var adminUser = HttpContext.Session.GetString("Username");
        var currentUserId = HttpContext.Session.GetInt32("UserId");
        var currentRoleId = HttpContext.Session.GetInt32("RoleId");

        if (string.IsNullOrWhiteSpace(adminUser))
            return Json(new { success = false, message = "Không xác định được tài khoản admin đang đăng nhập." });

        if (currentRoleId != 1)
            return Json(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });

        if (!currentUserId.HasValue)
            return Json(new { success = false, message = "Không xác định được người dùng hiện tại." });

        if (currentUserId.Value == model.UserId)
            return Json(new { success = false, message = "Không được tự đổi role của chính mình." });

        var result = _adminuserService.UpdateRoleSecure(
            model.UserId,
            model.RoleId,
            adminUser,
            model.AdminPassword
        );

        if (result == 1)
            return Json(new { success = true, message = "Đổi role thành công." });

        if (result == -1)
            return Json(new { success = false, message = "Sai mật khẩu admin." });

        if (result == -2)
            return Json(new { success = false, message = "Không tìm thấy tài khoản admin." });

        return Json(new { success = false, message = "Cập nhật role thất bại." });
    }
}