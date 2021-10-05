using GestaoEventos.Core.Aggregates.AuthAgg.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using GestaoEventos.Core.Aggregates.AuthAgg.Dtos;
using GestaoEventos.Core.SharedKernel.Dtos;
using GestaoEventos.Core.SharedKernel.Entities;

namespace GestaoEventos.Core.Aggregates.AuthAgg.Interfaces.Services
{
    public interface IUserService
    {
        Task<ResponseObject<UserForGetDto>> CreateUser(UserForRegisterDto userForRegisterDto);
        Task<ResponseObject<UserForGetDto>> UpdateUser(UserForEditDto userForEditDto);
        UserForGetDto GetUser(int idUser);
        FilterGenericDto<UserForGetDto> GetAllUsers(UserParametersDto userParametersDto);
        bool DeleteUser(int idUser);
        ResponseObject<bool> InactivateUser(int idUser);
        ResponseObject<bool> ActivateUser(int idUser);
        User GetUserByLogin(string loginUser);
        IEnumerable<int> GetUsersCells();
        Task<ResponseObject<ResponseUserFromStfCorpDto>> GetUserStfCorp(string loginUser);
    }
}
