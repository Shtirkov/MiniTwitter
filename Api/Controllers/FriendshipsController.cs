using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniTwitter.Interfaces;
using MiniTwitter.Mappers;
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
                return NotFound(new { Error = GlobalConstants.UserNotFoundErrorMessage });
            }

            var user = await _authService.GetUserAsync(User);

            if (friendToAdd.Id == user!.Id)
            {
                return BadRequest(new { Error = GlobalConstants.AddYourselfAsFriendErrorMessage });
            }

            var alreadyRequested = await _friendshipsService.CheckIfFriendshipIsRequestedAsync(user, friendToAdd);

            if (alreadyRequested)
            {
                return BadRequest(new { Error = GlobalConstants.FriendRequestAlreadySentErrorMessage });
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

                return Ok(friendship.ToFriendshipDto());
            }

            return NoContent();
        }

        [HttpPut("accept/{username}")]
        public async Task<IActionResult> AcceptFriendRequest(string username)
        {
            var user = await _authService.GetUserAsync(User);

            var friend = await _authService.FindUserByUsernameAsync(username);

            if (friend == null)
            {
                return NotFound(new { Error = GlobalConstants.UserNotFoundErrorMessage });
            }

            var request = await _friendshipsService.CheckForPendingFriendRequest(user!, friend);

            if (request == null)
            {
                return NotFound(new { message = GlobalConstants.NoPendingFriendRequestsErrorMessage });
            }

            request.IsConfirmed = true;

            await _friendshipsService.SaveChangesAsync();
            return Ok(request.ToFriendshipDto());
        }

        [HttpDelete("reject/{username}")]
        public async Task<IActionResult> RejectFriendRequest(string username)
        {
            var user = await _authService.GetUserAsync(User);

            var friend = await _authService.FindUserByUsernameAsync(username);

            if (friend == null)
            {
                return NotFound(new { Error = GlobalConstants.UserNotFoundErrorMessage });
            }

            var request = await _friendshipsService.CheckForPendingFriendRequest(user!, friend);

            if (request == null)
            {
                return NotFound(new { Error = GlobalConstants.NoPendingFriendRequestsErrorMessage });
            }

            _friendshipsService.Remove(request);
            await _friendshipsService.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("friends")]       
        public async Task<IActionResult> GetFriendsList()
        {
            var user = await _authService.GetUserAsync(User);

            var friends = await _friendshipsService.GetFriendsAsync(user!);
            var friendsDto = friends.Select(f => f.ToFriendshipDto());            

            return Ok(friendsDto);
        }
    }
}