using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniTwitter.Models;
using MiniTwitter.ResponseModels;

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

        [HttpPut("accept/{username}")]
        public async Task<IActionResult> AcceptFriendRequestAsync(string username)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(new { error = "User not logged in." });
            }

            var friend = await _context
                        .Users
                        .FirstOrDefaultAsync(u => u.UserName == username);

            if (friend == null)
            {
                return NotFound(new { error = "Such user does not exist" });
            }

            var request = await _context
                                .Friendships
                                .FirstOrDefaultAsync(x => x.UserId == friend.Id && x.FriendId == user.Id && !x.IsConfirmed);

            if (request == null)
            {
                return NotFound(new { message = "No pending request from this user." });
            }

            request.IsConfirmed = true;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Friend request accepted." });
        }

        [HttpDelete("reject/{username}")]
        public async Task<IActionResult> RejectFriendRequestAsync(string username)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(new { error = "User not logged in." });
            }

            var friend = await _context
                        .Users
                        .FirstOrDefaultAsync(u => u.UserName == username);

            if (friend == null)
            {
                return NotFound(new { error = "Such user does not exist" });
            }

            var request = await _context
                                .Friendships
                                .FirstOrDefaultAsync(x => x.UserId == friend.Id && x.FriendId == user.Id && !x.IsConfirmed);

            if (request == null)
            {
                return NotFound(new { message = "No pending request from this user." });
            }

            _context.Friendships.Remove(request);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("friends")]
        public async Task<IActionResult> GetFriendsListAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(new { error = "User not logged in." });
            }

            var friends = await _context
                        .Friendships
                        .Where((f => f.UserId == user.Id || f.FriendId == user.Id) && f.IsConfirmed)
                        .Select(f => new FriendDto
                        {
                            Email = f.Friend.Email!,
                            UserName = f.Friend.UserName!
                        })
                        .ToListAsync();

            return Ok(friends);
        }
    }
}
