using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestBlobStorage.Models.Dto;
using TestBlobStorage.Services;

namespace TestBlobStorage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        // isaaholic
        // pass:Mahal2003
        // id = 9cb1b314-cde0-4a15-4cdd-08dbc8edbedd AzureSql
        // id = b39c8ff2-435d-4074-6d52-08dbcc06c1de CosmosDb

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(CreateUserDto request)
        {
            try
            {
                return Ok(await _userService.Register(request));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(AuthUserDto request)
        {
            try
            {
                return Ok(await _userService.Login(request));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("getUser")]
        public async Task<ActionResult> GetUser(Guid userId)
        {
            try
            {
                return Ok(await _userService.GetUser(userId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("uploadPhoto")]
        [Authorize]
        public async Task<ActionResult> UploadImage([FromForm] UpdateProfilePictureDto request)
        {
            try
            {
                return Ok($"{await _userService.UploadProfilePicture(request)}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("updateUser")]
        [Authorize]
        public async Task<ActionResult> UpdateUser(UpdateUserDto request)
        {
            try
            {
                return Ok(await _userService.UpdateUser(request));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
