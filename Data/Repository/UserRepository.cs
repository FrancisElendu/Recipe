using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Recipe.Data.Repository.IRepository;
using Recipe.Models;
using Recipe.Models.DTOs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Recipe.Data.Repository
{
    public class UserRepository : Repository<UserManagerResponse<UserForRegisterDto>>, IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;

        public UserRepository(ApplicationDbContext db, UserManager<ApplicationUser> userManager
            , SignInManager<ApplicationUser> signInManager, IConfiguration config) : base(db) 
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }
        public async Task<UserManagerResponse<UserForLoginDto>> LoginUserAsync(UserForLoginDto userForLoginDto, IMailRepository mailRepository)
        {
            if (userForLoginDto == null)
                throw new NullReferenceException("Login model not found.");

            var user = await _userManager.FindByEmailAsync(userForLoginDto.Email);
            if (user == null)
            {
                return new UserManagerResponse<UserForLoginDto>
                {
                    Message = "User not found.",
                    IsSuccess = false
                };
            }
            var result = await _userManager.CheckPasswordAsync(user, userForLoginDto.Password);
                //.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);
            if (!result)
            {
                return new UserManagerResponse<UserForLoginDto>
                {
                    Message = "Invalid Password.",
                    IsSuccess = false
                };

            }

            await mailRepository.SendEmailAsync(userForLoginDto.Email, "New Login", "<h1>New Login Notice</h1><p>Account was logged into at " + DateTime.Now + "</p>");

            var generatedToken = GenerateJwToken(user);

            return new UserManagerResponse<UserForLoginDto>
            {
                Message = "Login Successful",
                IsSuccess = true,
                ExpiredDate = generatedToken.TokenExpiredDate,
                Token = generatedToken.Token
            };
        }

        public async Task<UserManagerResponse<UserForRegisterDto>> RegisterUserAsync(UserForRegisterDto userForRegisterDto, IMailRepository mailRepository)
        {
            if (userForRegisterDto==null)
            throw new NullReferenceException("Register model not found.");

            if (userForRegisterDto.Password != userForRegisterDto.ConfirmPassword)
                return new UserManagerResponse<UserForRegisterDto>
                {
                    Message = "Password and Confirm Password does not match",
                    IsSuccess = false
                };

            var userToCreate = new ApplicationUser
            {
                UserName = userForRegisterDto.Username,
                Email = userForRegisterDto.Email
            };

            //instead of using the identity provided createAsync function, you can use your own logic to register the user in the db
            var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password);

            if (result.Succeeded)
            {
                // TODO: You can add roles to user here



                var user = await _userManager.FindByEmailAsync(userForRegisterDto.Email);
                //TODO: Send confirmation email
                var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);

                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                string url = $"{_config["AppUrl"]}/api/auth/confirmemail?userId={user.Id}&token={validEmailToken}";

                await mailRepository.SendEmailAsync(user.Email, "Please Confirm your email", $"<h2>Welcome to Auth Demo</h2>" +
                    $"<p>Please confirm your email by <a href='{url}'>clicking here</a></p>");

                var userToDto = new UserForRegisterDto
                {
                    Username = user.UserName,
                    Email = user.Email
                };
                return new UserManagerResponse<UserForRegisterDto>
                {
                    SingleData = userToDto,
                    Message = "User Created Suceessfully.",
                    IsSuccess = true
                };
            }
            return new UserManagerResponse<UserForRegisterDto>
            {
                Message = "User not Created.",
                IsSuccess = false,
                Errors = result.Errors.Select(x=>x.Description)
            };
        }

        public async Task<UserManagerResponse<UserForRegisterDto>> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new UserManagerResponse<UserForRegisterDto>
                {
                    Message = "User not found.",
                    IsSuccess = false
                };
            var decodedToken = WebEncoders.Base64UrlDecode(token);
            var normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ConfirmEmailAsync(user, normalToken);
            if (result.Succeeded)
            {
                var userToDto = new UserForRegisterDto
                {
                    Username = user.UserName,
                    Email = user.Email
                };
                return new UserManagerResponse<UserForRegisterDto>
                {
                    SingleData = userToDto,
                    Message = "User email confirmed Suceessfully.",
                    IsSuccess = true
                };
            }
            return new UserManagerResponse<UserForRegisterDto>
            {
                Message = "User email not confirmed.",
                IsSuccess = false,
                Errors = result.Errors.Select(x => x.Description)
            };
        }


        private TokenCredentials GenerateJwToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("Email", user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow,
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var expireDate = token.ValidTo;
            string tokenToSend = tokenHandler.WriteToken(token);
            
            return new TokenCredentials
            {
                Token = tokenToSend,
                TokenExpiredDate = expireDate
            };
        }

        public async Task<UserManagerResponse<UserForLoginDto>> ForgetPasswordAsync(string email, IMailRepository mailRepository)
        {
            if (email == null)
                throw new NullReferenceException("email cannot be null.");

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new UserManagerResponse<UserForLoginDto>
                {
                    Message = "No user associated with this email",
                    IsSuccess = false
                };
            }

            var token = await  _userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = Encoding.UTF8.GetBytes(token);

            var validToken = WebEncoders.Base64UrlEncode(encodedToken);

            string url = $"{_config["AppUrl"]}/ResetPassword?email={email}&token={validToken}";

            await mailRepository.SendEmailAsync(user.Email, "Reset password", $"<h2>Follow the instruction to reset your password</h2>" +
                $"<p>To reset your password <a href='{url}'>click here</a></p>");

            return new UserManagerResponse<UserForLoginDto>
            {
                Message = "Reset password URL has been sent to your email Suceessfully.",
                IsSuccess = true
            };
        }


        public async Task<UserManagerResponse<ResetPasswordDto>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
                return new UserManagerResponse<ResetPasswordDto>
                {
                    IsSuccess = false,
                    Message = "No user associated with the email"
                };

            if(resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
                return new UserManagerResponse<ResetPasswordDto>
                {
                    IsSuccess = false,
                    Message = "Password does not match with confirm password"
                };

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

            if(result.Succeeded)
                return new UserManagerResponse<ResetPasswordDto>
                {
                    IsSuccess = true,
                    Message = "Password has been reset successfully"
                };

            return new UserManagerResponse<ResetPasswordDto>
            {
                IsSuccess = false,
                Message = "Password reset NOT successfully",
                Errors = result.Errors.Select(e => e.Description)
            };

        }

    }
}
