using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using TestBlobStorage.Data;
using TestBlobStorage.Models;
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
        // id = 9cb1b314-cde0-4a15-4cdd-08dbc8edbedd

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
