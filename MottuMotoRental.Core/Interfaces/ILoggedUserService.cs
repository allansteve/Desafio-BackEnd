using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MottuMotoRental.Core.Interfaces
{
    public interface ILoggedUserService
    {
        Guid? UserId { get; }
        IEnumerable<string>? Roles { get; }        
    }
}
