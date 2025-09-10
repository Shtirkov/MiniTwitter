using Microsoft.AspNetCore.Mvc;
using MiniTwitter.Entities;
using MiniTwitter.Interfaces;
using MiniTwitter.Mappers;
using MiniTwitter.ResponseModels;
using MiniTwitter.ViewModels;

namespace MiniTwitter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IFriendshipsService _friendshipsService;
        private readonly IPostsService _postsService;

        public PostsController(IAuthService authService, IFriendshipsService friendshipsService, IPostsService postsService)
        {
            _authService = authService;
            _friendshipsService = friendshipsService;
            _postsService = postsService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _authService.GetUserAsync(User);

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

            await _postsService.AddAsync(post);
            await _postsService.SaveChangesAsync();

            return Ok(new { message = "Post created!", postId = post.Id });
        }

        [HttpGet("user/{username}")]
        public async Task<IActionResult> GetPostsByUser(string username)
        {
            var user = await _authService.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(new { error = "User not logged in." });
            }

            var targetUser = await _authService.FindUserByUsernameAsync(username);
            if (targetUser == null)
            {
                return NotFound(new { error = "User not found." });
            }

            var isFriend = await _friendshipsService.CheckIfUsersAreFriendsAsync(user, targetUser);

            if (!isFriend && user != targetUser)
            {
                return Forbid();
            }

            var posts = await _postsService.GetPostsByUserAsync(username);

            var postsDto = posts          
                        .Select(p => new PostResponseDto
                        {
                            Id = p.Id,
                            Content = p.Content,
                            CreatedAt = p.CreatedAt,
                            Author = p.Author.UserName!,
                            Comments = p.Comments.Select(c => c.ToCommentDto()).ToList()
                        })
                        .ToList();


            return Ok(postsDto);
        }

        [HttpGet("feed")]
        public async Task<IActionResult> PopulateFeed()
        {
            var user = await _authService.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(new { error = "User not logged in." });
            }

            var friends = await _friendshipsService.GetFriendsAsync(user);
            var friendNames = friends.Select(f => f.UserId == user.Id ? f.FriendId : f.UserId);

            var postsToDisplay = await _postsService.GetFriendsPosts(user, friends);

            if (postsToDisplay.Any())
            {
                var postsDto = postsToDisplay.Select(p => new PostResponseDto
                {
                    Id = p.Id,
                    Content = p.Content,
                    Author = p.Author!.UserName!,
                    CreatedAt = p.CreatedAt,
                    Comments = p.Comments.Select(c => c.ToCommentDto()).ToList()
                });

                return Ok(postsDto);
            }

            var defaultPosts = new List<PostResponseDto>()
            {
                new PostResponseDto
                {
                    Id = -1,
                    Content = "Default post 1",
                    Author = "System"
                },
                new PostResponseDto
                {
                    Id= -2,
                    Content = "Default post 2",
                    Author = "System"
                },
                new PostResponseDto
                {
                    Id= -3,
                    Content = "Default post 3",
                    Author = "System"
                },
                new PostResponseDto
                {
                    Id= -4,
                    Content = "Default post 4",
                    Author= "System"
                },
                new PostResponseDto
                {
                    Id= -5,
                    Content = "Default post 5",
                    Author = "System"
                }
            };

            return Ok(defaultPosts);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeletePost([FromRoute] int id)
        {
            var user = await _authService.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(new { error = "User not logged in." });
            }

            var post = await _postsService.GetPostAsync(id);

            if (post == null)
            {
                return NotFound(new { error = "There is no such post." });
            }

            if (post.AuthorId != user.Id)
            {
                return Forbid();
            }

            _postsService.Remove(post);
            await _postsService.SaveChangesAsync();

            return NoContent();
        }
    }
}
