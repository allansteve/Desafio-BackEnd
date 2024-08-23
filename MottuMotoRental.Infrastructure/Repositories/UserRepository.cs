using MottuMotoRental.Core.Entities;
using MottuMotoRental.Infrastructure.Data;
using MottuMotoRental.Infrastructure.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MottuMotoRental.Infrastructure.Repositories
{
    public interface IUserRepository : IRepository<SystemUser>
    {
    }

    public class UserRepository : Repository<SystemUser>, IUserRepository
    {
        public UserRepository(MotoRentalContext context) : base(context)
        {

        }

    }
}
