using Microsoft.AspNetCore.Identity;
using MottuMotoRental.Core.Entities;
using MottuMotoRental.Infrastructure.Data.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MottuMotoRental.Infrastructure.Data.Seed
{

    public static class DataBaseSeed
    {
        public static async Task SeedAsync(UserManager<SystemUser>? userManager, RoleManager<SystemRole>? roleManager)
        {
            if (userManager is not null && roleManager is not null)
            {
                await SeedRolesAsync(roleManager);
                await SeedUserAsync(userManager);
            }
        }

        public static async Task SeedRolesAsync(RoleManager<SystemRole> roleManager)
        {
            await roleManager.CreateAsync(new SystemRole(SystemRolesConst.SystemAdmin));
            await roleManager.CreateAsync(new SystemRole(SystemRolesConst.SystemDelivery));
        }

        public static async Task SeedUserAsync(UserManager<SystemUser> userManager)
        {
            var adminUser = new SystemUser
            {   
                UserName = "Administrator",
                Email = "dev.admin@mottu.com.br",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var deliveryUser = new SystemUser
            {   
                UserName = "Delivery_User",
                Email = "delivery.user@mottu.com.br",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var user = await userManager.FindByEmailAsync(adminUser.Email);
            if (user == null)
            {
                await userManager.CreateAsync(adminUser, "987Pa$$w0rd.");
                await userManager.AddToRoleAsync(adminUser, SystemRolesConst.SystemAdmin);
            }
            var delivUser = await userManager.FindByEmailAsync(deliveryUser.Email);
            if (delivUser == null)
            {
                await userManager.CreateAsync(deliveryUser, "987Pa$$w0rd.");                
                await userManager.AddToRoleAsync(deliveryUser, SystemRolesConst.SystemDelivery);
            }
        }


    }
}
