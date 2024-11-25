using Microsoft.AspNetCore.Mvc;

namespace NoSQLSocialNetwork.ViewComponents
{
	public class RightSidebarViewComponent:ViewComponent
	{
		public IViewComponentResult Invoke()
		{
			return View();
		}
	}

}
