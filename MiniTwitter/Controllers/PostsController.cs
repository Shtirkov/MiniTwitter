using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniTwitter.Entities;
using MiniTwitter.Helpers;
using MiniTwitter.Interfaces;
using MiniTwitter.Mappers;
using MiniTwitter.RequestModels;
using MiniTwitter.ResponseModels;
using MiniTwitter.ViewModels;

namespace MiniTwitter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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

            var post = new Post
            {
                Author = user!,
                Content = model.Content,
            };

            await _postsService.AddAsync(post);
            await _postsService.SaveChangesAsync();

            return Ok(post.ToPostDto());
        }

        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditPost(int id, EditPostRequestDto postRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _authService.GetUserAsync(User);

            var post = await _postsService.GetPostAsync(id);

            if (post == null)
            {
                return NotFound(new { Error = GlobalConstants.PostNotFoundErrorMessage });
            }

            if (post.AuthorId != user!.Id)
            {
                return Forbid();
            }

            _postsService.Edit(post, postRequestDto);
            await _postsService.SaveChangesAsync();

            return Ok(post.ToPostDto());
        }

        [HttpGet("user/{username}")]
        public async Task<IActionResult> GetPostsByUser(string username, [FromQuery] QueryParams queryParams)
        {
            var user = await _authService.GetUserAsync(User);
           
            var targetUser = await _authService.FindUserByUsernameAsync(username);
            if (targetUser == null)
            {
                return NotFound(new { Error = GlobalConstants.UserNotFoundErrorMessage });
            }

            var isFriend = await _friendshipsService.CheckIfUsersAreFriendsAsync(user!, targetUser);

            if (!isFriend && user != targetUser)
            {
                return Forbid();
            }

            var posts = await _postsService.GetPostsByUserAsync(username, queryParams);

            var postsDto = posts
                        .Items
                        .Select(p => p.ToPostDto())
                        .ToList();


            var result = new PagedResult<PostResponseDto>
            {
                Items = postsDto,
                TotalCount = posts.TotalCount,
                Page = posts.Page,
                PageSize = posts.PageSize
            };

            return Ok(result);
        }

        [HttpGet("feed")]
        public async Task<IActionResult> PopulateFeed([FromQuery] QueryParams queryParams)
        {
            var user = await _authService.GetUserAsync(User);

            var friends = await _friendshipsService.GetFriendsAsync(user!);
            var friendNames = friends.Select(f => f.UserId == user!.Id ? f.FriendId : f.UserId);

            var postsToDisplay = await _postsService.GetFriendsPosts(user!, friends, queryParams);

            if (postsToDisplay.Items.Count > 0)
            {
                var postsDto = postsToDisplay
                    .Items
                    .Select(p => p.ToPostDto())
                    .ToList();

                var result = new PagedResult<PostResponseDto>
                {
                    Items = postsDto,
                    TotalCount = postsToDisplay.TotalCount,
                    Page = postsToDisplay.Page,
                    PageSize = postsToDisplay.PageSize
                };

                return Ok(result);
            }

            var defaultPosts = new List<PostResponseDto>
            {
                new PostResponseDto { Id = -1, Content = "Default post 1", Author = "System" },
                new PostResponseDto { Id = -2, Content = "Default post 2", Author = "System" },
                new PostResponseDto { Id = -3, Content = "Default post 3", Author = "System" },
                new PostResponseDto { Id = -4, Content = "Default post 4", Author = "System" },
                new PostResponseDto { Id = -5, Content = "Default post 5", Author = "System" }
            };

            var defaultResult = new PagedResult<PostResponseDto>
            {
                Items = defaultPosts,
                TotalCount = defaultPosts.Count,
                Page = 1,
                PageSize = defaultPosts.Count
            };

            return Ok(defaultResult);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeletePost([FromRoute] int id)
        {
            var user = await _authService.GetUserAsync(User);

            var post = await _postsService.GetPostAsync(id);

            if (post == null)
            {
                return NotFound(new { Error = GlobalConstants.PostNotFoundErrorMessage });
            }

            if (post.AuthorId != user!.Id)
            {
                return Forbid();
            }

            _postsService.Remove(post);
            await _postsService.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("like/{id}")]
        public async Task<IActionResult> LikePost(int id)
        {
            var user = await _authService.GetUserAsync(User);
           
            var post = await _postsService.GetPostAsync(id);

            if (post == null)
            {
                return NotFound(new { Error = GlobalConstants.PostNotFoundErrorMessage });
            }

            if (!await _friendshipsService.CheckIfUsersAreFriendsAsync(user!, post.Author))
            {
                return Forbid();
            }

            await _postsService.Like(post, user!);
            await _postsService.SaveChangesAsync();

            return Ok(post.ToPostDto());
        }
    }
}
