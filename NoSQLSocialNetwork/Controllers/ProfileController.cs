using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using NoSQLSocialNetwork.Data;
using NoSQLSocialNetwork.Entities;
using NoSQLSocialNetwork.ViewModels;
using System.Security.Claims;

namespace NoSQLSocialNetwork.Controllers
{
	public class ProfileController : Controller
	{
        private readonly IMongoCollection<User>? _users;
        public ProfileController(MongoDbService mongoDbService)
        {
            _users = mongoDbService.Database?.GetCollection<User>("Users");
        }
        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var claimsPrincipal = User as ClaimsPrincipal;
                var userIdClaim = claimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim != null)
                {
                    var userId = ObjectId.Parse(userIdClaim.Value);
                    var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();

                    if (user != null)
                    {
                        var result = new
                        {
                            Avatar = user.AvatarUrl,
                            Name = user.FullName
                        };
                        var posts = user.Posts ?? new List<Post>();
                        var sortedPosts = posts.OrderByDescending(post => post.CreatedAt);
                        List<PostVM> postsVM = new List<PostVM>();
                        foreach (var post in sortedPosts)
                        {
                            bool isLiked = post.Likes != null && post.Likes.Contains(userId);
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
                                Comments = post.Comments,
                                IsLiked = isLiked
                            });
                        }
                        ViewBag.Posts = postsVM;
                        return View(result);
                    }
                }
            }

            return View(null);
        }
    }
}
