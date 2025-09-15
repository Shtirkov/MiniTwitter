using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniTwitter.Interfaces;
using MiniTwitter.Mappers;
using MiniTwitter.Models;
using MiniTwitter.RequestModels;

namespace MiniTwitter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentsService _commentsService;
        private readonly IAuthService _authService;
        private readonly IPostsService _postsService;

        public CommentsController(ICommentsService commentsService, IAuthService authService, IPostsService postsService)
        {
            _commentsService = commentsService;
            _authService = authService;
            _postsService = postsService;
        }

        [HttpPost("post/{postId}")]
        public async Task<IActionResult> AddComment([FromBody] CreateCommentRequestDto model, int postId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _authService.GetUserAsync(User);

            var post = await _postsService.GetPostAsync(postId);

            if (post == null)
            {
                return NotFound(new { Error = GlobalConstants.PostNotFoundErrorMessage });
            }

            var comment = new Comment
            {
                AuthorId = user!.Id,
                Author = user,
                PostId = postId,
                Post = post,
                Content = model.Content
            };

            await _commentsService.AddAsync(comment);
            await _commentsService.SaveChangesAsync();

            return Ok(comment.ToCommentDto());
        }

        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var user = await _authService.GetUserAsync(User);            

            var comment = await _commentsService.GetAsync(commentId);

            if (comment == null)
            {
                return NotFound(new {Error = GlobalConstants.CommentNotFoundErrorMessage});
            }

            if (comment.AuthorId != user!.Id)
            {
                return Forbid();
            }

            _commentsService.Remove(comment);
            await _commentsService.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{commentId}")]
        public async Task<IActionResult> EditComment(int commentId, EditCommentRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _authService.GetUserAsync(User);
            
            var comment = await _commentsService.GetAsync(commentId);

            if (comment == null)
            {
                return NotFound(new { Error = GlobalConstants.CommentNotFoundErrorMessage });
            }

            if (comment.AuthorId != user!.Id)
            {
                return Forbid();
            }

            _commentsService.Edit(comment, model);
            await _commentsService.SaveChangesAsync();

            return Ok(comment.ToCommentDto());
        }
    }
}
