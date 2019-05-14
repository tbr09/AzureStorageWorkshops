using Microsoft.AspNetCore.Mvc;

namespace AzureStorageMVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
