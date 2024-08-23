using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MottuMotoRental.Core.DTOs.User
{
    public class CreateUserDto
    {
        public string Name { get; set; } = "";
        public string Password { get; set; } = "";
        public string Email { get; set; } = "";
        public List<string> Roles { get; set; } = new();
    }
}
