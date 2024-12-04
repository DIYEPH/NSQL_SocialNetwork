﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using NoSQLSocialNetwork.Data;
using NoSQLSocialNetwork.Entities;
using NoSQLSocialNetwork.ViewModels;
using System.Security.Claims;

namespace NoSQLSocialNetwork.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PostController : ControllerBase
	{
		private readonly IMongoCollection<User>? _users;
		public PostController(MongoDbService mongoDbService)
		{
			_users = mongoDbService.Database?.GetCollection<User>("Users");
		}
		[HttpPost("create")]
		public async Task<IActionResult> CreatePost([FromForm] CreatePostVM postForm)
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

					// Xử lý tải lên hình ảnh
					var imageUrls = new List<string>();
					if (postForm.Images != null && postForm.Images.Any())
					{
						foreach (var file in postForm.Images)
						{
							if (file.Length > 0)
							{
								var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
								var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);
								using (var stream = new FileStream(filePath, FileMode.Create))
								{
									await file.CopyToAsync(stream);
								}
								var fileUrl = $"/uploads/{fileName}";
								imageUrls.Add(fileUrl);  // Thêm URL của mỗi hình vào danh sách
							}
						}
					}

					// Tạo bài đăng mới với tất cả các hình ảnh
					var post = new Post
					{
						AuthorId = userId,
						Content = postForm.Content,
						ImageUrls = imageUrls,  // Chứa tất cả URL của hình ảnh
						CreatedAt = DateTime.UtcNow,
						UpdatedAt = DateTime.UtcNow
					};

					var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
					if (user != null)
					{
						if (user.Posts == null)
						{
							user.Posts = new List<Post>();  
						}
						var update = Builders<User>.Update.Push(u => u.Posts, post);
						await _users.UpdateOneAsync(u => u.Id == userId, update);
					}

					return Ok(new { message = "Post created successfully!" });
				}
			}
			return Unauthorized();
		}
	}
}