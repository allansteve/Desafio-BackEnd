using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using MottuMotoRental.Core.Configuration;
using MottuMotoRental.Core.DTOs.User;
using MottuMotoRental.Core.Entities;
using MottuMotoRental.Core.Interfaces;
using System.Security.Claims;

namespace MottuMotoRental.Core.Services
{
    public class AuthService
    {
        private readonly SignInManager<SystemUser> _signInManager;
        private readonly UserManager<SystemUser> _userManager;
        private readonly ILoggedUserService _loggedUser;

        public AuthService(
            SignInManager<SystemUser> signInManager,
            UserManager<SystemUser> userManager,
            ILoggedUserService loggedUserService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _loggedUser = loggedUserService;   
        }

        public async Task<bool> CreateUser(CreateUserDto user)
        {
            var systemUser = MapUser(user.Name, user.Email);
            var result = await _userManager.CreateAsync(systemUser, user.Password);
            var rolesResult = await _userManager.AddToRolesAsync(systemUser, user.Roles);

            return result.Succeeded && rolesResult.Succeeded;
        }

        public SystemUser MapUser(string name, string email)
        {
            return new SystemUser
            {
                UserName = name,
                Email = email
            };
        }

        public async Task<string> CreateTokenAsync(SystemUser user)
        {
            var claims = await GetClaims(user);

            return JwtConfiguration.GenerateToken(claims);
        }



        private async Task<ClaimsIdentity> GetClaims(SystemUser user)
        {
            var claims = new ClaimsIdentity(
                new Claim[]
                {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),                
                },
                JwtBearerDefaults.AuthenticationScheme
            );

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
                claims.AddClaim(new Claim(ClaimTypes.Role, role));

            return claims;
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();            
        }

      
        public async Task<SystemUser?> FindCurrentUserAsync()
        {
            var userId = _loggedUser?.UserId?.ToString() ?? "";
            if (string.IsNullOrEmpty(userId))
                return null;
            var user = await _userManager.FindByIdAsync(userId);
            return user;
        }   

        public async Task<SystemUser?> FindUserByEmail(string mail)
        {
            return await _userManager.FindByEmailAsync(mail);

        }


        public async Task<bool> CheckPasswordAsync(SystemUser user, string password)
        {
            var canLogin = await _signInManager.CheckPasswordSignInAsync(user, password, false);

            if (canLogin.Succeeded)
                return true;
            return false;
        }

    }
}
