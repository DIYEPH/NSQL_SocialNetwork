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
                        // Lấy danh sách người theo dõi
                        var listFollowings = user.Followings ?? new List<ObjectId>();
                        listFollowings.Add(user.Id);

                        // Lấy danh sách người dùng liên quan
                        var users = await _users.Find(u => listFollowings.Contains(u.Id)).ToListAsync();

                        // Chuẩn bị danh sách bài viết
                        var posts = new List<PostVM>();

                        foreach (var u in users)
                        {
                            var postsUser = u.Posts ?? new List<Post>();

                            foreach (var post in postsUser)
                            {
                                bool isLiked = post.Likes != null && post.Likes.Contains(userId);

                                // Chuẩn bị danh sách bình luận
                                var comments = new List<CommentVM>();
                                foreach (var comment in post.Comments ?? new List<Comment>())
                                {
                                    var userComment = await _users.Find(uc => uc.Id == comment.UserId).FirstOrDefaultAsync();
                                    if (userComment != null)
                                    {
                                        comments.Add(new CommentVM
                                        {
                                            Id = comment.Id,
                                            UserId = userComment.Id,
                                            FullName = userComment.FullName,
                                            Content = comment.Content,
                                            Status = comment.Status,
                                            AvatarUrl = userComment.AvatarUrl,
                                            CreateAt = comment.CreateAt,
                                        });
                                    }
                                }

                                // Sắp xếp bình luận theo thời gian
                                var sortedComments = comments.OrderByDescending(c => c.CreateAt).ToList();

                                // Thêm bài viết vào danh sách
                                posts.Add(new PostVM
                                {
                                    Id = post.Id,
                                    AuthorId = post.AuthorId,
                                    AuthorAvatar = u.AvatarUrl,
                                    AuthorName = u.FullName,
                                    Content = post.Content,
                                    ImageUrls = post.ImageUrls,
                                    Likes = post.Likes,
                                    Status = post.Status,
                                    CreatedAt = post.CreatedAt,
                                    UpdatedAt = post.UpdatedAt,
                                    IsLiked = isLiked,
                                    Comments = sortedComments
                                });
                            }
                        }

                        // Sắp xếp bài viết theo thời gian
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
