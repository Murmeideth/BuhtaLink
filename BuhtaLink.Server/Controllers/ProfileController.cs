using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BuhtaLink.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProfileController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userIdClaim = User.FindFirst("userId");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            return Unauthorized();

        var user = await _db.Users.FindAsync(userId);
        if (user == null)
            return NotFound();

        return Ok(new UserDto(user));
    }

    [HttpGet("friends")]
    public async Task<IActionResult> GetFriends()
    {
        var userIdClaim = User.FindFirst("userId");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            return Unauthorized();

        var friendships = await _db.Friendships
            .Where(f => (f.UserId == userId || f.FriendId == userId) && f.Status == "accepted")
            .Include(f => f.User)
            .Include(f => f.Friend)
            .ToListAsync();

        var friends = friendships.Select(f =>
        {
            var friend = f.UserId == userId ? f.Friend : f.User;
            return new FriendDto
            {
                Id = friend.Id,
                Name = friend.Nickname ?? friend.Username,
                Avatar = friend.AvatarUrl ?? "personalplaceholder.jpg"
            };
        }).ToList();

        return Ok(friends);
    }

    [HttpGet("posts")]
    public async Task<IActionResult> GetPosts(int skip = 0, int take = 20)
    {
        var userIdClaim = User.FindFirst("userId");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            return Unauthorized();

        var posts = await _db.Posts
            .Where(p => p.AuthorId == userId)
            .Include(p => p.Author)
            .OrderByDescending(p => p.CreatedAt)
            .Skip(skip)
            .Take(take)
            .Select(p => new PostDto
            {
                Id = p.Id,
                UserName = p.Author.Nickname ?? p.Author.Username,
                PostText = p.Content,
                CommentsCount = 0, // Пока нет комментариев
                PostDate = p.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
                AvatarSource = p.Author.AvatarUrl ?? "personalplaceholder.jpg"
            })
            .ToListAsync();

        return Ok(posts);
    }
}

// DTO модели
public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string FullName { get; set; }
    public string Nickname { get; set; }
    public string AvatarUrl { get; set; }
    public DateTime? LastSeen { get; set; }

    public UserDto(User user)
    {
        Id = user.Id;
        Username = user.Username;
        FullName = user.FullName;
        Nickname = user.Nickname;
        AvatarUrl = user.AvatarUrl;
        LastSeen = user.LastSeen;
    }
}

public class FriendDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Avatar { get; set; }
}

public class PostDto
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string PostText { get; set; }
    public int CommentsCount { get; set; }
    public string PostDate { get; set; }
    public string AvatarSource { get; set; }
}