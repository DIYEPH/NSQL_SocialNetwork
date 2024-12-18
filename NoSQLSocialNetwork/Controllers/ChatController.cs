using Microsoft.AspNetCore.Mvc;

namespace NoSQLSocialNetwork.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
