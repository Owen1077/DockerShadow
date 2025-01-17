using DockerShadow.Core.Contract;
using DockerShadow.Core.DTO.Request;
using DockerShadow.Core.DTO.Response;
using DockerShadow.Domain.Common;
using DockerShadow.Domain.QueryParameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DockerShadow.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrator")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService accountService)
        {
            _userService = accountService;
        }
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult<Response<AuthenticationResponse>>> AuthenticateAsync(AuthenticationRequest request)
        {
            return Ok(await _userService.AuthenticateAsync(request, HttpContext.RequestAborted));
        }
        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<ActionResult<Response<string>>> Logout()
        {
            return Ok(await _userService.LogoutAsync());
        }
        [HttpGet("getUsers")]
        public async Task<ActionResult<PagedResponse<List<UserResponse>>>> GetUsers([FromQuery] UserQueryParameters queryParameters)
        {
            return Ok(await _userService.GetUsersAsync(queryParameters, HttpContext.RequestAborted));
        }
        [HttpGet("getUser/{id}")]
        public async Task<ActionResult<Response<UserResponse>>> GetUserById(string id)
        {
            return Ok(await _userService.GetUserById(id, HttpContext.RequestAborted));
        }
        [HttpPost("addUser")]
        public async Task<ActionResult<Response<string>>> AddUser(AddUserRequest request)
        {
            return Ok(await _userService.AddUserAsync(request, HttpContext.RequestAborted));
        }
        [HttpPost("editUser")]
        public async Task<ActionResult<Response<string>>> EditUser([FromBody] EditUserRequest request)
        {
            return Ok(await _userService.EditUserAsync(request, HttpContext.RequestAborted));
        }
        [HttpPost("deleteUser")]
        public async Task<ActionResult<Response<string>>> DeleteUser([FromBody] DeleteUserRequest request)
        {
            return Ok(await _userService.DeleteUserAsync(request, HttpContext.RequestAborted));
        }
        [AllowAnonymous]
        [HttpPost("resetUser")]
        public async Task<ActionResult<Response<string>>> ResetUser([FromBody] ResetUserRequest request)
        {
            return Ok(await _userService.ResetUserAsync(request));
        }
        [HttpPost("resetUserLockout")]
        public async Task<ActionResult<Response<string>>> ResetUserLockout([FromBody] ResetUserRequest request)
        {
            return Ok(await _userService.ResetUserLockoutAsync(request));
        }
        [AllowAnonymous]
        [HttpPost("changePasswordWithToken")]
        public async Task<ActionResult<Response<string>>> PasswordReset([FromBody] ChangePasswordRequest request)
        {
            return Ok(await _userService.ChangePasswordWithToken(request));
        }
    }
}