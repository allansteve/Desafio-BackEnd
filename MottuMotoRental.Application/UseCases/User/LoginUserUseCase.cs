using Amazon.Runtime.Internal;
using MottuMotoRental.Application.DTOs;
using MottuMotoRental.Core.Entities;
using MottuMotoRental.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MottuMotoRental.Application.UseCases.User
{
    public class LoginUserUseCase
    {
        private readonly AuthService _authService;
        public LoginUserUseCase(AuthService authService)
        {
            _authService = authService; 
        }

        public async Task<LoginResponse> Execute(LoginDto login)
        {
            var user = await _authService.FindUserByEmail(login.Email);
            if (user == null)
                LoginResponse.UserNotFound();
            var res = await _authService.CheckPasswordAsync(user,login.Password);
            if(!res)
                LoginResponse.InvalidCredentials();

            var token = await _authService.CreateTokenAsync(user);  

            return LoginResponse.Success(token);
        }


    }
}
