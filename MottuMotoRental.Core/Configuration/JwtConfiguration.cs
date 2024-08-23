using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MottuMotoRental.Core.Configuration
{
    public static class JwtConfiguration
    {
        private static SigningCredentials SigningCredentials { get; set; } = null!;
        private static IConfigurationSection JwtConfig { get; set; } = null!;

        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtConfig = configuration.GetSection("JwtConfig");

            ArgumentNullException.ThrowIfNull(jwtConfig, nameof(jwtConfig));
            JwtConfig = jwtConfig;

            var secretKey = jwtConfig["secret"];
            ArgumentException.ThrowIfNullOrEmpty(secretKey, nameof(secretKey));

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            SigningCredentials = new SigningCredentials(
                symmetricSecurityKey,
                SecurityAlgorithms.HmacSha256
            );

            services
                .AddAuthentication(opt =>
                {
                    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtConfig["validIssuer"],
                        IssuerSigningKey = symmetricSecurityKey
                    };
                });
        }

        public static string GenerateToken(ClaimsIdentity claimsIdentity)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Issuer = JwtConfig["validIssuer"],
                Expires = DateTime.UtcNow.AddHours(Convert.ToDouble(JwtConfig["expiresInHours"])),
                SigningCredentials = SigningCredentials
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

}
