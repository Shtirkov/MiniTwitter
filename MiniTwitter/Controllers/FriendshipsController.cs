using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniTwitter.Models;

namespace MiniTwitter.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class FriendshipsController : ControllerBase
    {

        private readonly TwitterContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FriendshipsController(TwitterContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("send/{username}")]
        public async Task<IActionResult> SendFriendRequestAsync(string username)
        {
            var friendToAdd = await _context
                        .Users
                        .FirstOrDefaultAsync(x => x.UserName == username);

            if (friendToAdd == null)
            {
                return NotFound(new { error = "Such user do not exist." });
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(new { error = "User not logged in." });
            }

            if (friendToAdd.Id == user.Id)
            {
                return BadRequest(new { error = "You cannot add yourself as a friend." });
            }



            var alreadyRequested = await _context
                                    .Friendships.AnyAsync(f => (f.UserId == user.Id && f.FriendId == friendToAdd.Id) ||
                                    (f.UserId == friendToAdd.Id && f.FriendId == user.Id));

            if (alreadyRequested)
            {
                return BadRequest(new { error = "Friend request already exists." });
            }

            var areFriends = await _context
                .Friendships
                .AnyAsync(f => f.UserId == user.Id && f.FriendId == friendToAdd.Id && f.IsConfirmed
                || f.UserId == friendToAdd.Id && f.FriendId == user.Id && f.IsConfirmed);

            if (!areFriends)
            {
                var friendship = new Friendship
                {
                    UserId = user.Id,
                    User = user,
                    FriendId = friendToAdd.Id,
                    Friend = friendToAdd,
                    IsConfirmed = false
                };

                _context.Friendships.Add(friendship);
                await _context.SaveChangesAsync();

                return Ok(new { friendToAdd.Id, friendToAdd.UserName });
            }

            return NoContent();

        }
    }
}
