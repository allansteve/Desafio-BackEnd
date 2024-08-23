using MottuMotoRental.Core.Interfaces;
using MottuMotoRental.Infrastructure.Repositories.Base;
using MottuMotoRental.Infrastructure.Repositories;
using MottuMotoRental.Core.Services;
using Microsoft.AspNetCore.Identity;
using MottuMotoRental.Core.Entities;
using MottuMotoRental.Infrastructure.Data;
using MottuMotoRental.Application.UseCases.User;
using MottuMotoRental.API.Services;
using MottuMotoRental.Core.Configuration;

namespace MottuMotoRental.API.Configuration
{
    public static class ConfigurationInjection
    {

        public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddBusinessDependencies(configuration);
            services.AddInfrastructureDependencies(configuration);
            services.AddRepositoriesDependencies(configuration);
            services.ConfigureJwt(configuration);   
        }

        public static void AddBusinessDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<AuthService>();
            services
                   .AddIdentity<SystemUser, SystemRole>(options =>
                   {
                       options.User.RequireUniqueEmail = true;
                   })
           .AddEntityFrameworkStores<MotoRentalContext>()
           .AddDefaultTokenProviders();
            services.AddScoped<ListUserUseCase>(); 
            services.AddScoped<LoginUserUseCase>();

            services.AddScoped<ILoggedUserService, LoggedUserService>();
        }

        public static void AddInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        public static void AddRepositoriesDependencies(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<IUserRepository, UserRepository>();
        }

    }
}
