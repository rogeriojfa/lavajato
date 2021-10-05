using System.Collections.Generic;
using GestaoEventos.Core.Aggregates.AuthAgg.Dtos;
using GestaoEventos.Core.Aggregates.AuthAgg.Entities;
using GestaoEventos.Core.SharedKernel.Dtos;
using GestaoEventos.Core.SharedKernel.Interfaces.Repositories;

namespace GestaoEventos.Core.Aggregates.AuthAgg.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        User GetByLogin(string loginUser);
        IEnumerable<int> GetUsersCells();
        FilterGenericDto<UserForGetDto> FilterUsers(UserParametersDto userParametersDto);
    }
}
