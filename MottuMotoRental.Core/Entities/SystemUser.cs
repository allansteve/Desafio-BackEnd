using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MottuMotoRental.Core.Entities
{
    public partial class SystemUser : IdentityUser<Guid>
    {

    }
    public class SystemRole : IdentityRole<Guid>
    {
        public SystemRole()
        {
            Id = Guid.NewGuid();
        }
        public SystemRole(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }
    }
}
