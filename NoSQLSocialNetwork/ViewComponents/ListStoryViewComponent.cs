using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using NoSQLSocialNetwork.Data;
using NoSQLSocialNetwork.Entities;
using System.Security.Claims;

namespace NoSQLSocialNetwork.ViewComponents
{
	public class ListStoryViewComponent: ViewComponent
	{
		private readonly IMongoCollection<User>? _users;
		public ListStoryViewComponent(MongoDbService mongoDbService)
		{
			_users = mongoDbService.Database?.GetCollection<User>("Users");
		}
		public async Task<IViewComponentResult> InvokeAsync()
		{
			if (User.Identity?.IsAuthenticated == true)
			{
				var claimsPrincipal = User as ClaimsPrincipal;
				var userIdClaim = claimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier);

				if (userIdClaim != null)
				{
					var userId = ObjectId.Parse(userIdClaim.Value);
					var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();

					var result = new
					{
						Avatar = user?.AvatarUrl,
						Name = user?.FullName
					};

					return View(result);
				}
			}
			return View(null);
		}
	}
}
