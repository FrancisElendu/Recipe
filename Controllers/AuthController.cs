using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Recipe.Data.Repository.IRepository;
using Recipe.Models.DTOs;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Recipe.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Recipe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(IUnitOfWork unitOfWork, IConfiguration config, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _unitOfWork.UserServices.RegisterUserAsync(userForRegisterDto, _unitOfWork.MailServices);
                if (result.IsSuccess)
                    return Ok(result);
                return BadRequest(result);
            }
            return BadRequest("Model not valid");
        }

        //api/auth/confirmemail?userid&token
        [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return NotFound();

            var result = await _unitOfWork.UserServices.ConfirmEmailAsync(userId, token);

            if (result.IsSuccess)
            {
                return Redirect($"{_config["AppUrl"]}/confirmemail.html");
            }
            return BadRequest(result);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _unitOfWork.UserServices.LoginUserAsync(userForLoginDto, _unitOfWork.MailServices);
                if (result.IsSuccess)
                    return Ok(result);
                return Unauthorized();
            }
            return BadRequest("Model not valid");
        }

        //api/auth/forgetpassword
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return NotFound();

            var result = await _unitOfWork.UserServices.ForgetPasswordAsync(email, _unitOfWork.MailServices);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        //api/auth/resetpassword
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPasword(ResetPasswordDto resetPasswordDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _unitOfWork.UserServices.ResetPasswordAsync(resetPasswordDto);
                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
            }
            return BadRequest("Model not valid");
        }
    }
}