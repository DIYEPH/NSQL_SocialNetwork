using Microsoft.AspNetCore.Mvc;

namespace NoSQLSocialNetwork.ViewComponents
{
	public class LeftSidebarViewComponent:ViewComponent
	{
		public IViewComponentResult Invoke()
		{
			return View();
		}
	}
}
