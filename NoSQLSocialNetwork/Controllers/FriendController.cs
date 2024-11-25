using Microsoft.AspNetCore.Mvc;

namespace NoSQLSocialNetwork.Controllers
{
	public class FriendController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
