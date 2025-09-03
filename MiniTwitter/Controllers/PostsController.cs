using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniTwitter.Entities;
using MiniTwitter.Models;
using MiniTwitter.ViewModels;

namespace MiniTwitter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly TwitterContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostsController(TwitterContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePostAsync([FromBody] CreatePostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(new { error = "User not logged in." });
            }

            var post = new Post
            {
                Author = user,
                Content = model.Content,
                CreatedAt = DateTime.UtcNow
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Post created!", postId = post.Id });
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPostsByUserAsync(string userId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            var isFriend = await _context
                .Friendships
                .AnyAsync(f => f.UserId == user.Id && f.FriendId == userId && f.IsConfirmed 
                || f.UserId == userId && f.FriendId == user.Id && f.IsConfirmed);            

            if (!isFriend)
            {
                return Forbid();
            }

            var posts = await _context
                        .Posts
                        .Where(p => p.AuthorId == userId)
                        .OrderByDescending(p => p.CreatedAt)
                        .Select(p => new
                        {
                            p.Id,
                            p.Content,
                            p.CreatedAt,
                            Author = p.Author.UserName
                        })
                        .ToListAsync();

            return Ok(posts);
        }

        [HttpDelete("delete/{postId}")]
        public async Task<IActionResult> DeletePostAsync(int postId)
        {
            var post = await _context
                            .Posts
                            .FirstOrDefaultAsync(p => p.Id == postId)
        }
    }
}
