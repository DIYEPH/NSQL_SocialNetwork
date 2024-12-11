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

        public IActionResult Index()
		{
			return View();
		}
        [HttpPost("AddFriend")]
        public async Task<IActionResult> AddFriend(string friendId)
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
                    var friend = await _users.Find(u => u.Id == ObjectId.Parse(friendId)).FirstOrDefaultAsync();
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
                    user.Followings.Add(friend.Id);
                    await _users.ReplaceOneAsync(u => u.Id == userId, user);
                    return StatusCode(StatusCodes.Status200OK, new { message = "Friend added." });
                }
            }
            return StatusCode(StatusCodes.Status401Unauthorized, new { message = "Unauthorized." });
        }
        [HttpPost("AcceptFriend")]
        public async Task<IActionResult> AcceptFriend(string friendId)
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
                    var friend = await _users.Find(u => u.Id == ObjectId.Parse(friendId)).FirstOrDefaultAsync();
                    if (friend == null)
                    {
                        return StatusCode(StatusCodes.Status404NotFound, new { message = "Friend not found." });
                    }
                    var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
                    if (user == null)
                    {
                        return StatusCode(StatusCodes.Status404NotFound, new { message = "User not found." });
                    }
                    if (user.Followers == null)
                    {
                        user.Followers = new List<ObjectId>();
                    }
                    if (user.Followers.Contains(friend.Id))
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, new { message = "Friend already exists." });
                    }
                    user.Followers.Add(friend.Id);
                    await _users.ReplaceOneAsync(u => u.Id == userId, user);
                    return StatusCode(StatusCodes.Status200OK, new { message = "Friend added." });
                }
            }
            return StatusCode(StatusCodes.Status401Unauthorized, new { message = "Unauthorized." });
        }
    }
}
