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
					var listFollowings = user?.Followings ?? new List<ObjectId>();
                    if (user != null)
					{
						listFollowings.Add(user.Id);
						var users = await _users.Find(u => listFollowings.Contains(u.Id)).ToListAsync();
						var posts = new List<PostVM>();
                        foreach (var u in users)
                        {
                            // Lấy bài viết của người dùng này
                            var postsUser = u.Posts ?? new List<Post>(); // Nếu không có bài viết, tạo một danh sách rỗng
                            foreach (var post in postsUser)
                            {
                                bool isLiked = post.Likes != null && post.Likes.Contains(userId); // Kiểm tra xem người dùng có thích bài viết này không
                                posts.Add(new PostVM
                                {
                                    Id = post.Id,
                                    AuthorId = post.AuthorId,
                                    AuthorAvatar = u.AvatarUrl, // Avatar của người dùng này
                                    AuthorName = u.FullName, // Tên người dùng này
                                    Content = post.Content,
                                    ImageUrls = post.ImageUrls,
                                    Likes = post.Likes,
                                    Status = post.Status,
                                    CreatedAt = post.CreatedAt,
                                    UpdatedAt = post.UpdatedAt,
                                    //Comments = post.Comments,
                                    IsLiked = isLiked
                                });
                            }
                        }
                        var sortedPosts = posts.OrderByDescending(p => p.CreatedAt).ToList();

                        return View(sortedPosts);
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
