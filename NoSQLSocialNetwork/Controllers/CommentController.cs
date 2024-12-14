using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using NoSQLSocialNetwork.Data;
using NoSQLSocialNetwork.Entities;
using NoSQLSocialNetwork.ViewModels;
using System.Security.Claims;

namespace NoSQLSocialNetwork.Controllers
{
    public class CommentController : Controller
    {
        private readonly IMongoCollection<User>? _users;
        public CommentController(MongoDbService mongoDbService)
        {
            _users = mongoDbService.Database?.GetCollection<User>("Users");
        }
        public async Task<IActionResult> Index(string postId)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var claimsPrincipal = User as ClaimsPrincipal;
                var userIdClaim = claimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier);

                if (_users == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Database connection error." });
                }

                if (userIdClaim != null)
                {
                    var userId = ObjectId.Parse(userIdClaim.Value);

                    // Lấy thông tin người dùng hiện tại
                    var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
                    ViewBag.User = user;

                    if (user != null)
                    {
                        // Tìm tất cả các bài post có id trùng với postId trong toàn bộ collection User
                        var filter = Builders<User>.Filter.ElemMatch(u => u.Posts, p => p.Id == ObjectId.Parse(postId));
                        var foundUser = await _users.Find(filter).FirstOrDefaultAsync();

                        if (foundUser != null)
                        {
                            var post = foundUser.Posts?.FirstOrDefault(p => p.Id == ObjectId.Parse(postId));

                            if (post != null)
                            {
                                bool isLiked = post.Likes != null && post.Likes.Contains(userId);

                                var postVM = new PostVM
                                {
                                    Id = post.Id,
                                    AuthorId = post.AuthorId,
                                    AuthorAvatar = foundUser.AvatarUrl,
                                    AuthorName = foundUser.FullName,
                                    Content = post.Content,
                                    ImageUrls = post.ImageUrls,
                                    Likes = post.Likes,
                                    Status = post.Status,
                                    CreatedAt = post.CreatedAt,
                                    UpdatedAt = post.UpdatedAt,
                                    Comments = new List<CommentVM>(),
                                    IsLiked = isLiked,

                                };
                                foreach (var comment in post.Comments ?? new List<Comment>())
                                {
                                    var commenter = await _users.Find(u => u.Id == comment.UserId).FirstOrDefaultAsync();
                                    var commentVM = new CommentVM
                                    {
                                        UserId = comment.UserId,
                                        FullName = commenter.FullName,
                                        AvatarUrl = commenter.AvatarUrl,
                                        Content = comment.Content,
                                        CreateAt = comment.CreateAt,
                                        Status = comment.Status
                                    };
                                    postVM.Comments.Add(commentVM);
                                }


                                return View(postVM);
                            }
                        }
                    }
                }
            }

            return View(null);
        }
        [HttpPost("Comment/AddComment")]
        public async Task<IActionResult> AddComment(string postId)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var claimsPrincipal = User as ClaimsPrincipal;
                var userIdClaim = claimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier);
                if (_users == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Database connection error." });
                }
                if (userIdClaim != null)
                {
                    var userId = ObjectId.Parse(userIdClaim.Value);
                    var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
                    if (user != null)
                    {
                        var filter = Builders<User>.Filter.ElemMatch(u => u.Posts, p => p.Id == ObjectId.Parse(postId));
                        var foundUser = await _users.Find(filter).FirstOrDefaultAsync();
                        if (foundUser != null)
                        {
                            var post = foundUser.Posts?.FirstOrDefault(p => p.Id == ObjectId.Parse(postId));
                            if (post != null)
                            {
                                var comment = new Comment
                                {
                                    UserId = userId,
                                    Content = Request.Form["content"],
                                    CreateAt = DateTime.Now,
                                    Status = 1
                                };
                                post.Comments ??= new List<Comment>();
                                post.Comments.Add(comment);
                                var update = Builders<User>.Update.Set(u => u.Posts, foundUser.Posts);
                                await _users.UpdateOneAsync(u => u.Id == foundUser.Id, update);
                                return RedirectToAction("Index", new { postId = postId });
                            }
                        }
                    }
                }
            }
            return RedirectToAction("Index", new { postId = postId });
        }
    }
}
