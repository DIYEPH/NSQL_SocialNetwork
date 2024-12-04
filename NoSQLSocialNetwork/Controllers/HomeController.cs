using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using NoSQLSocialNetwork.Data;
using NoSQLSocialNetwork.Entities;
using NoSQLSocialNetwork.Models;
using NoSQLSocialNetwork.ViewModels;
using System.Diagnostics;
using System.Security.Claims;

namespace NoSQLSocialNetwork.Controllers
{
	[Authorize]
	public class HomeController : Controller
    { 
		private readonly IMongoCollection<User>? _users;

        public HomeController(MongoDbService mongoDbService)
        {
			_users = mongoDbService.Database?.GetCollection<User>("Users");
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
				var claimsPrincipal = User as ClaimsPrincipal;
				var userIdClaim = claimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier);

				if (_users == null)
					return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Database connection error." });

				if (userIdClaim != null)
				{
					var userId = ObjectId.Parse(userIdClaim.Value);
					var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
					if (user != null)
					{
						var posts = user.Posts ?? new List<Post>();
						var sortedPosts = posts.OrderByDescending(post => post.CreatedAt);
						List<PostVM> postsVM = new List<PostVM>();
						foreach (var post in sortedPosts)
							postsVM.Add(new PostVM
							{
								Id = post.Id,
								AuthorId = post.AuthorId,
								AuthorAvatar = user.AvatarUrl,
								AuthorName = user.FullName,
								Content = post.Content,
								ImageUrls = post.ImageUrls,
								Likes = post.Likes,
								Status = post.Status,
								CreatedAt = post.CreatedAt,
								UpdatedAt = post.UpdatedAt,
								Comments = post.Comments
							});
						return View(postsVM);
					}
				}
			}
			return Unauthorized();
		}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
