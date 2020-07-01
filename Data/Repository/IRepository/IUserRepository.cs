using Recipe.Models;
using Recipe.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recipe.Data.Repository.IRepository
{
    public interface IUserRepository : IRepository<UserManagerResponse<UserForRegisterDto>>
    {
        Task<UserManagerResponse<UserForRegisterDto>> RegisterUserAsync(UserForRegisterDto userForRegisterDto, IMailRepository mailRepository);
        Task<UserManagerResponse<UserForRegisterDto>> ConfirmEmailAsync(string userId, string token);

        Task<UserManagerResponse<UserForLoginDto>> LoginUserAsync(UserForLoginDto userForLoginDto, IMailRepository mailRepository);

        //Task<UserManagerResponse<UserForRegisterDto>> ForgetPasswordAsync(string email);
        //Task<UserManagerResponse<UserForLoginDto>> ForgetPasswordAsync(UserForLoginDto userForLoginDto, IMailRepository mailRepository);
        Task<UserManagerResponse<UserForLoginDto>> ForgetPasswordAsync(string email, IMailRepository mailRepository);

        Task<UserManagerResponse<ResetPasswordDto>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}
