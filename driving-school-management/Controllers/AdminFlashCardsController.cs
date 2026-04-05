using driving_school_management.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace driving_school_management.Controllers
{
    public class AdminFlashCardsController : Controller
    {
        private readonly FlashCardOracleHelper _flashCardHelper;

        public AdminFlashCardsController(IConfiguration configuration)
        {
            _flashCardHelper = new FlashCardOracleHelper(configuration);
        }

        public IActionResult Index()
        {
            var data = _flashCardHelper.GetSummary();
            return View(data);
        }

        public IActionResult Details(int id)
        {
            var vm = _flashCardHelper.GetBySignId(id);

            if (vm == null || vm.Items == null || !vm.Items.Any())
            {
                TempData["Error"] = "Biển báo này chưa có dữ liệu flashcard từ học viên.";
                return RedirectToAction(nameof(Index));
            }

            return View(vm);
        }
    }
}