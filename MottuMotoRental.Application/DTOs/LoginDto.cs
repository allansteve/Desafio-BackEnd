using MongoDB.Bson.Serialization.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MottuMotoRental.Application.DTOs
{
    public class LoginDto
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class LoginResponse
    {
        public LoginResult Result { get; set; }
        public string Token { get; set; } = "";

        public static LoginResponse Success(string token) => new() { Result = LoginResult.Success,Token=token };
        public static LoginResponse InvalidCredentials() => new() { Result = LoginResult.InvalidCredentials };
        public static LoginResponse UserNotFound() => new() { Result = LoginResult.UserNotFound };  

    }
    public enum LoginResult
    {
        Success,
        InvalidCredentials,
        UserNotFound
    }   


}
