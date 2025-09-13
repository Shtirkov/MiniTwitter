using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniTwitter.Interfaces;
using MiniTwitter.Models;
using MiniTwitter.ResponseModels;

namespace MiniTwitter.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FriendshipsController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IFriendshipsService _friendshipsService;

        public FriendshipsController(IAuthService authService, IFriendshipsService friendshipsService)
        {
            _authService = authService;
            _friendshipsService = friendshipsService;
        }

        [HttpPost("send/{username}")]
        public async Task<IActionResult> SendFriendRequest(string username)
        {
            var friendToAdd = await _authService.FindUserByUsernameAsync(username);

            if (friendToAdd == null)
            {
                return NotFound(new { error = "Such user do not exist." });
            }

            var user = await _authService.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(new { error = "User not logged in." });
            }

            if (friendToAdd.Id == user.Id)
            {
                return BadRequest(new { error = "You cannot add yourself as a friend." });
            }

            var alreadyRequested = await _friendshipsService.CheckIfFriendshipIsRequestedAsync(user, friendToAdd);

            if (alreadyRequested)
            {
                return BadRequest(new { error = "Friend request already exists." });
            }

            var areFriends = await _friendshipsService.CheckIfUsersAreFriendsAsync(user, friendToAdd);

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

                await _friendshipsService.AddAsync(friendship);
                await _friendshipsService.SaveChangesAsync();

                return Ok(new { friendToAdd.Id, friendToAdd.UserName });
            }

            return NoContent();

        }

        [HttpPut("accept/{username}")]
        public async Task<IActionResult> AcceptFriendRequest(string username)
        {
            var user = await _authService.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(new { error = "User not logged in." });
            }

            var friend = await _authService.FindUserByUsernameAsync(username);

            if (friend == null)
            {
                return NotFound(new { error = "Such user does not exist" });
            }

            var request = await _friendshipsService.CheckForPendingFriendRequest(user, friend);

            if (request == null)
            {
                return NotFound(new { message = "No pending request from this user." });
            }

            request.IsConfirmed = true;

            await _friendshipsService.SaveChangesAsync();
            return Ok(new { message = "Friend request accepted." });
        }

        [HttpDelete("reject/{username}")]
        public async Task<IActionResult> RejectFriendRequest(string username)
        {
            var user = await _authService.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(new { error = "User not logged in." });
            }

            var friend = await _authService.FindUserByUsernameAsync(username);

            if (friend == null)
            {
                return NotFound(new { error = "Such user does not exist" });
            }

            var request = await _friendshipsService.CheckForPendingFriendRequest(user, friend);

            if (request == null)
            {
                return NotFound(new { message = "No pending request from this user." });
            }

            _friendshipsService.Remove(request);
            await _friendshipsService.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("friends")]       
        public async Task<IActionResult> GetFriendsList()
        {
            var user = await _authService.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(new { error = "User not logged in." });
            }

            var friends = await _friendshipsService.GetFriendsAsync(user);
            var friendsDto = friends.Select(f => new FriendResponseDto
            {
                UserName = f.UserId == user.Id ? f.Friend.UserName! : f.User.UserName!,
                Email = f.UserId == user.Id ? f.Friend.Email! : f.User.Email!
            });

            return Ok(friendsDto);
        }
    }
}