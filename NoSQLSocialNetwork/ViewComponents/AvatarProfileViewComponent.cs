using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using NoSQLSocialNetwork.Data;
using NoSQLSocialNetwork.Entities;
using System.Security.Claims;

namespace NoSQLSocialNetwork.ViewComponents
{
	public class AvatarProfileViewComponent : ViewComponent
	{
		private readonly IMongoCollection<User>? _users;
		public AvatarProfileViewComponent(MongoDbService mongoDbService)
		{
			_users = mongoDbService.Database?.GetCollection<User>("Users");
		}
		public IViewComponentResult Invoke()
		{
			if (User.Identity?.IsAuthenticated == true)
			{
				var claimsPrincipal = User as ClaimsPrincipal;
				var userIdClaim = claimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier);

				if (userIdClaim != null)
				{
					var userId = ObjectId.Parse(userIdClaim.Value); // Nếu userId là ObjectId
					var user = _users?.Find(u => u.Id == userId).FirstOrDefault();

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
