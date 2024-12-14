using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using NoSQLSocialNetwork.Data;
using NoSQLSocialNetwork.Entities;
using System.Security.Claims;

namespace NoSQLSocialNetwork.Controllers
{
    public class FriendController : Controller
	{
        private readonly IMongoCollection<User>? _users;
        public FriendController(MongoDbService mongoDbService)
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
                    var followers = user?.Followers ?? new List<ObjectId>();
                    var followings = user?.Followings ?? new List<ObjectId>();

                    if (followings != null && followers != null)
                    {
                        // Lấy danh sách bạn bè (người dùng có trong followings hoặc followers)
                        var friends = await _users.Find(u => followings.Contains(u.Id) && followers.Contains(u.Id)).ToListAsync();
                        var followingusers = await _users.Find(u => followings.Contains(u.Id) && !followers.Contains(u.Id)).ToListAsync();
                        // Kiểm tra những người trong followers mà chưa có trong followings (người chưa follow lại)
                        var invitationFriendIds = followers?.Where(f => !followings.Contains(f)).ToList();

                        // Nếu có người trong followers mà chưa có trong followings, thêm vào ViewBag
                        if (invitationFriendIds?.Any() == true)
                        {
                            var invitationFriends = await _users.Find(u => invitationFriendIds.Contains(u.Id)).ToListAsync();
                            ViewBag.Invitation = invitationFriends;
                        }

                        // Thêm danh sách bạn bè vào ViewBag
                        ViewBag.Friend = friends;
                        ViewBag.FollowingUsers = followingusers;

                        // Tìm những người dùng không có trong cả followers và followings
                        var allUsers = await _users.Find(u => true).ToListAsync(); // Lấy tất cả người dùng
                        var usersNotFollowed = allUsers
                            .Where(u => u.Id != user?.Id 
                             && !(followers?.Contains(u.Id) ?? false) 
                             && !(followings?.Contains(u.Id) ?? false)) 
                            .ToList();

                        // Thêm vào ViewBag nếu có
                        if (usersNotFollowed.Any())
                        {
                            ViewBag.UsersNotFollowed = usersNotFollowed;
                        }
                    }
                }
            }

            return View();
        }
        [HttpPost("Friend/AddFriend")]
        public async Task<IActionResult> AddFriend([FromBody] FriendRequestModel model)
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
                    var friend = await _users.Find(u => u.Id == ObjectId.Parse(model.UserId)).FirstOrDefaultAsync();
                    if (friend == null)
                    {
                        return StatusCode(StatusCodes.Status404NotFound, new { message = "Friend not found." });
                    }
                    var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
                    if (user == null)
                    {
                        return StatusCode(StatusCodes.Status404NotFound, new { message = "User not found." });
                    }
                    if (user.Followings == null)
                    {
                        user.Followings = new List<ObjectId>();
                    }
                    if (friend.Followers == null)
                    {
                        friend.Followers = new List<ObjectId>();
                    }
                    if (user.Followings.Contains(friend.Id))
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, new { message = "Friend already exists." });
                    }
                    user.Followings.Add(friend.Id);
                    friend.Followers.Add(user.Id);
                    await _users.ReplaceOneAsync(u => u.Id == userId, user);
                    await _users.ReplaceOneAsync(u => u.Id == friend.Id, friend);
                    return StatusCode(StatusCodes.Status200OK, new { message = "Friend added." });
                }
            }
            return StatusCode(StatusCodes.Status401Unauthorized, new { message = "Unauthorized." });
        }
        [HttpPost("Friend/AcceptFriend")]
        public async Task<IActionResult> AcceptFriend([FromBody] FriendRequestModel model)
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
                    var friend = await _users.Find(u => u.Id == ObjectId.Parse(model.UserId)).FirstOrDefaultAsync();
                    if (friend == null)
                    {
                        return StatusCode(StatusCodes.Status404NotFound, new { message = "Friend not found." });
                    }
                    var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
                    if (user == null)
                    {
                        return StatusCode(StatusCodes.Status404NotFound, new { message = "User not found." });
                    }
                    if (user.Followings == null)
                    {
                        user.Followings = new List<ObjectId>();
                    }
                    if (user.Followings.Contains(friend.Id))
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, new { message = "Friend already exists." });
                    }
                    if (friend.Followers == null)
                    {
                        friend.Followers = new List<ObjectId>();
                    }
                    user.Followings.Add(friend.Id);
                    friend.Followers.Add(user.Id);
                    await _users.ReplaceOneAsync(u => u.Id == userId, user);
                    await _users.ReplaceOneAsync(u => u.Id == friend.Id, friend);
                    return StatusCode(StatusCodes.Status200OK, new { message = "Friend added." });
                }
            }
            return StatusCode(StatusCodes.Status401Unauthorized, new { message = "Unauthorized." });
        }
    }
}
public class FriendRequestModel
{
    public string? UserId { get; set; } 
}