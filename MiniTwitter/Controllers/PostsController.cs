using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniTwitter.Entities;
using MiniTwitter.Migrations;
using MiniTwitter.Models;
using MiniTwitter.ResponseModels;
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
        public async Task<IActionResult> CreatePostAsync([FromBody] CreatePost model)
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

        [HttpGet("user/{username}")]
        public async Task<IActionResult> GetPostsByUserAsync(string username)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(new { error = "User not logged in." });
            }

            var targetUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (targetUser == null)
            {
                return NotFound(new { error = "User not found." });
            }

            var isFriend = await _context.Friendships.AnyAsync(f =>
                (f.UserId == user.Id && f.FriendId == targetUser.Id ||
                 f.UserId == targetUser.Id && f.FriendId == user.Id)
                && f.IsConfirmed);

            if (!isFriend && user != targetUser)
            {
                return Forbid();
            }

            var posts = await _context
                        .Posts
                        .Where(p => p.Author.UserName == username)
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

        [HttpGet("feed")]
        public async Task<IActionResult> PopulateFeed()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(new { error = "User not logged in." });
            }

            var friends = await _context
                           .Friendships
                           .Where(f => (f.UserId == user.Id || f.FriendId == user.Id) && f.IsConfirmed)
                           .Select(f => f.UserId == user.Id ? f.FriendId : f.UserId)
                           .ToListAsync();

            var postsToDisplay = await _context
                                .Posts
                                .Where(p => friends.Contains(p.AuthorId) && p.AuthorId != user.Id)
                                .OrderByDescending(p => p.CreatedAt)
                                .Select(p => new PostDto
                                {
                                    Id = p.Id,
                                    Content = p.Content,
                                    Author = p.Author.UserName!,
                                    CreatedAt = p.CreatedAt
                                })
                                .ToListAsync();

            if (postsToDisplay.Any())
            {
                return Ok(postsToDisplay);
            }

            var defaultPosts = new List<PostDto>()
            {
                new PostDto
                {
                    Id = -1,
                    Content = "Default post 1",
                    Author = "System"
                },
                new PostDto
                {
                    Id= -2,
                    Content = "Default post 2",
                    Author = "System"
                },
                new PostDto
                {
                    Id= -3,
                    Content = "Default post 3",
                    Author = "System"
                },
                new PostDto
                {
                    Id= -4,
                    Content = "Default post 4",
                    Author= "System"
                },
                new PostDto
                {
                    Id= -5,
                    Content = "Default post 5",
                    Author = "System"
                }
            };

            return Ok(defaultPosts);
        }

        [HttpDelete("delete/{postId}")]
        public async Task<IActionResult> DeletePostAsync(int postId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(new { error = "User not logged in." });
            }

            var post = await _context
                            .Posts
                            .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
            {
                return NotFound(new { error = "There is no such post." });
            }

            if (post.Author.Id != user.Id)
            {
                return Forbid();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
