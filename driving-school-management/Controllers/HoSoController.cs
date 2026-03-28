using driving_school_management.Models;
using Microsoft.AspNetCore.Mvc;

public class HoSoController : Controller
{
    private readonly HoSoService _service;

    public HoSoController(HoSoService service)
    {
        _service = service;
    }

    public IActionResult Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Auth");
        }

        var data = _service.GetMyHoSoPage(userId.Value);
        return View(data);
    }

    public IActionResult Detail(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Auth");
        }

        var data = _service.GetDetailByUser(id, userId.Value);
        if (data == null)
        {
            return NotFound();
        }

        return PartialView("_Detail", data);
    }
}