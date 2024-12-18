using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using NoSQLSocialNetwork.Data;
using NoSQLSocialNetwork.Entities;
using NoSQLSocialNetwork.ViewModels;
using System.Security.Claims;

namespace NoSQLSocialNetwork.Controllers
{
    public class SearchController : Controller
    {
        private readonly IMongoCollection<User>? _users;
        public SearchController(MongoDbService mongoDbService)
        {
            _users = mongoDbService.Database?.GetCollection<User>("Users");
        }
        public async Task<IActionResult> Index(string textsearch)
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
                    var followers = user?.Followers ?? new List<ObjectId>();

                    // Lấy danh sách bạn bè
                    var filter = Builders<User>.Filter.And(
                        Builders<User>.Filter.In(u => u.Id, listFollowings),
                        Builders<User>.Filter.In(u => u.Id, followers)
                    );

                    // Nếu có textsearch, thêm điều kiện tìm theo FullName
                    if (!string.IsNullOrEmpty(textsearch))
                    {
                        filter = filter & Builders<User>.Filter.Regex(u => u.FullName, new MongoDB.Bson.BsonRegularExpression(textsearch, "i")); // Tìm không phân biệt chữ hoa chữ thường
                    }

                    var friends = await _users.Find(filter).ToListAsync();
                    var followingUsers = await _users.Find(u => listFollowings.Contains(u.Id) && !followers.Contains(u.Id)).ToListAsync();

                    // Thêm danh sách bạn bè vào ViewBag
                    ViewBag.Friend = friends;
                    ViewBag.FollowingUsers = followingUsers;

                    // Tìm những người chưa được theo dõi
                    var allUsers = await _users.Find(u => true).ToListAsync();
                    var usersNotFollowed = allUsers
                        .Where(u => u.Id != user?.Id
                                    && !followers.Contains(u.Id)
                                    && !listFollowings.Contains(u.Id))
                        .ToList();

                    // Thêm vào ViewBag nếu có người dùng chưa theo dõi
                    if (usersNotFollowed.Any())
                    {
                        ViewBag.UsersNotFollowed = usersNotFollowed;
                    }
                    if (user != null)
                        listFollowings.Add(user.Id);
                    var usersToFetchPosts = await _users.Find(u => listFollowings.Contains(u.Id)).ToListAsync();
                    var posts = new List<PostVM>();

                    foreach (var u in usersToFetchPosts)
                    {
                        var postsUser = u.Posts ?? new List<Post>();
                        foreach (var post in postsUser)
                        {
                            // Lọc bài viết theo nội dung nếu có textsearch
                            if (!string.IsNullOrEmpty(textsearch) && post.Content != null && !post.Content.Contains(textsearch, StringComparison.OrdinalIgnoreCase))
                            {
                                continue; // Nếu không khớp nội dung, bỏ qua bài viết này
                            }

                            bool isLiked = post.Likes != null && post.Likes.Contains(userId); // Kiểm tra nếu người dùng thích bài viết này
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
                                IsLiked = isLiked
                            });
                        }
                    }

                    // Sắp xếp các bài viết theo ngày tạo
                    var sortedPosts = posts.OrderByDescending(p => p.CreatedAt).ToList();

                    // Trả về View với danh sách bài viết đã lọc và sắp xếp
                    return View(sortedPosts);
                }
            }
            return Unauthorized();
        }

    }
}
