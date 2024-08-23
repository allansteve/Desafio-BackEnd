using MongoDB.Driver.Core.Operations;
using MottuMotoRental.Core.Entities;
using MottuMotoRental.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MottuMotoRental.Application.UseCases.User
{
    public class ListUserUseCase
    {
        IUserRepository _userRepository;
        public ListUserUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<SystemUser>> Execute()
        {
            var users=await _userRepository.GetAllAsync();
            return users.ToList();
        }
    }
}
