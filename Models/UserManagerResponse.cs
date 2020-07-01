using Microsoft.IdentityModel.Tokens;
using Recipe.Data.Repository;
using Recipe.Data.Repository.IRepository;
using Recipe.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recipe.Models
{
    public class UserManagerResponse<T> where T:class
    {
        public string Message { get; set; }

        public bool IsSuccess { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public IEnumerable<T> Data { get; set; }
        public T SingleData { get; set; }
        public TokenCredentials tokenCredentials { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public string Token { get; set; }
    }

    public class TokenCredentials : SecurityToken
    {
        public DateTime? TokenExpiredDate { get; set; }
        public string Token { get; set; }

        public override string Id { get; }

        public override string Issuer { get; }

        public override SecurityKey SecurityKey { get; }

        public override SecurityKey SigningKey { get; set; }

        public override DateTime ValidFrom { get; }

        public override DateTime ValidTo { get; }
    }
    //public class RegisterApllicationUserResponse
    //{
    //    public RegisterApllicationUserResponse()
    //    {
    //        RegisterApplicationUser = new UserManagerResponse<UserForRegisterDto>();
    //    }
    //    public UserManagerResponse<UserForRegisterDto> RegisterApplicationUser { get; set; }
    //}

    //public class LoginApllicationUserResponse
    //{
    //    public LoginApllicationUserResponse()
    //    {
    //        LoginApplicationUser = new UserManagerResponse<UserForLoginDto>();
    //    }
    //    public UserManagerResponse<UserForLoginDto> LoginApplicationUser { get; set; }
    //}
}
