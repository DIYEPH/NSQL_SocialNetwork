using Microsoft.AspNetCore.Mvc;

namespace NoSQLSocialNetwork.Controllers
{
	public class ProfileController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
