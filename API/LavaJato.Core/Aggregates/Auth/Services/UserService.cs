using System;
using AutoMapper;
using GestaoEventos.Core.Aggregates.AuthAgg.Dtos;
using GestaoEventos.Core.Aggregates.AuthAgg.Entities;
using GestaoEventos.Core.Aggregates.AuthAgg.Interfaces.Repositories;
using GestaoEventos.Core.Aggregates.AuthAgg.Interfaces.Services;
using GestaoEventos.Core.SharedKernel.Dtos;
using GestaoEventos.Core.SharedKernel.Entities;
using GestaoEventos.Core.SharedKernel.Interfaces.Services;
using GestaoEventos.Core.SharedKernel.Interfaces.UoW;
using GestaoEventos.Core.SharedKernel.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoEventos.Core.Aggregates.AuthAgg.Services
{
    public class UserService : IUserService
    { 
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IUnityOfWork _unityOfWork;
        private readonly IStfCorpIntegrationService _stfCorpIntegrationService;
        private readonly IProfileRepository _profileRepository;

        public UserService(IUserRepository userRepository, IUnityOfWork unityOfWork, IMapper mapper, IStfCorpIntegrationService stfCorpIntegrationService, IProfileRepository profileRepository)
        {
            _userRepository = userRepository;
            _unityOfWork = unityOfWork;
            _mapper = mapper;
            _stfCorpIntegrationService = stfCorpIntegrationService;
            _profileRepository = profileRepository;
        }

        public async Task<ResponseObject<UserForGetDto>> CreateUser(UserForRegisterDto userForRegisterDto)
        {
            var profile = _profileRepository.GetById(userForRegisterDto.IdProfile);
            if (profile == null)
                return new ResponseObject<UserForGetDto>(false, "There is no Profile with the given id");

            var loginExistsStfCorp = await GetUserStfCorp(userForRegisterDto.Login);
            if (!loginExistsStfCorp.Success)
            {
                return new ResponseObject<UserForGetDto>(false, "There is no login or the user is already created"); 
            }

            var cellToCreate = ConvertCellResponseFromStfCorp(loginExistsStfCorp.Object.Celula);
            if (!(userForRegisterDto.Name == loginExistsStfCorp.Object.NomeCompleto &&
                userForRegisterDto.Cell == cellToCreate && userForRegisterDto.Email == loginExistsStfCorp.Object.Email))
            {
                return new ResponseObject<UserForGetDto>(false, "User information does not match with the owner of this login");
            }

            var user = _mapper.Map<User>(userForRegisterDto);
            _userRepository.Create(user);
            var commit = _unityOfWork.Commit();

            return commit
                ? new ResponseObject<UserForGetDto>(true, obj: _mapper.Map<UserForGetDto>(user))
                : new ResponseObject<UserForGetDto>(false);
        }

        public async Task<ResponseObject<UserForGetDto>> UpdateUser(UserForEditDto userForEditDto)
        {
            var profile = _profileRepository.GetById(userForEditDto.IdProfile);
            if (profile == null)
                return new ResponseObject<UserForGetDto>(false, "There is no Profile with the given id");

            var user = _userRepository.GetById(userForEditDto.Id);
            if (user == null)
            {
                return new ResponseObject<UserForGetDto>(false, "There is no User with the given id");
            }

            if (userForEditDto.Login != user.Login)
            {
                return new ResponseObject<UserForGetDto>(false, "Login does not match with Database");
            }

            var responseStfCorp = await _stfCorpIntegrationService.GetUserStfCorp(userForEditDto.Login);
            if (responseStfCorp.Success)
            {
                var cellToCreate = ConvertCellResponseFromStfCorp(responseStfCorp.Object.Celula);

                user.Name = responseStfCorp.Object.NomeCompleto;
                user.Cell = cellToCreate;
                user.Email = responseStfCorp.Object.Email;
            }

            user.IdProfile = userForEditDto.IdProfile;
            user.ChangeDate = userForEditDto.ChangeDate;
            user.IdUserWhoChange = userForEditDto.IdUserWhoChange;

            _userRepository.Update(user);
            var commit = _unityOfWork.Commit();

            return commit
                ? new ResponseObject<UserForGetDto>(true, obj: _mapper.Map<UserForGetDto>(user))
                : new ResponseObject<UserForGetDto>(false);
        }

        public UserForGetDto GetUser(int idUser)
        {
            var user = _userRepository.GetByIdWithIncludes(idUser, true, x => x.Profile);
            var userForGetDto = _mapper.Map<UserForGetDto>(user);
            return userForGetDto;
        }

        public FilterGenericDto<UserForGetDto> GetAllUsers(UserParametersDto userParametersDto)
        {
            var users = _userRepository.FilterUsers(userParametersDto);

            return users;
        }

        public bool DeleteUser(int idUser)
        {
            _userRepository.Delete(idUser);
            var commit = _unityOfWork.Commit();

            return commit;
        }

        public ResponseObject<bool> InactivateUser(int idUser)
        {
            var userForCheckId = _userRepository.GetById(idUser);
            if (userForCheckId == null)
            {
                return new ResponseObject<bool>(false, "There is no user with the given id", false);
            }

            var user = new User { Id = idUser, Active = false };
            _userRepository.Inactivate(user);

            var commit = _unityOfWork.Commit();
            return new ResponseObject<bool>(commit, obj: commit);
        }

        public ResponseObject<bool> ActivateUser(int idUser)
        {
            var userForCheckId = _userRepository.GetById(idUser);
            if (userForCheckId == null)
            {
                return new ResponseObject<bool>(false, "There is no user with the given id", false);
            }

            var user = new User { Id = idUser, Active = true };
            _userRepository.Inactivate(user);

            var commit = _unityOfWork.Commit();
            return new ResponseObject<bool>(commit, obj: commit);
        }

        public User GetUserByLogin(string loginUser)
        {
            var user = _userRepository.GetByLogin(loginUser);
            return user;
        }

        public IEnumerable<int> GetUsersCells()
        {
            var cells = _userRepository.GetUsersCells();
            return cells;
        }

        public async Task<ResponseObject<ResponseUserFromStfCorpDto>> GetUserStfCorp(string loginUser)
        {
            var response = await _stfCorpIntegrationService.GetUserStfCorp(loginUser);
            if (!response.Success) return response;

            var user = _userRepository.GetByLogin(response.Object.Login);
            if (user != null)
            {
                if (user.Active == false)
                    return new ResponseObject<ResponseUserFromStfCorpDto>(false, Errors.CREATE_USER_DB_INACTIVE);
                return new ResponseObject<ResponseUserFromStfCorpDto>(false, Errors.CREATE_USER_DB);
            }

            return response;
        }

        private static int ConvertCellResponseFromStfCorp(string cellStfCorp)
        {
            var getOnlyDigitsFromAString = cellStfCorp.Where(char.IsDigit);
            var stringWithDigits = string.Concat(getOnlyDigitsFromAString);
            var digitsParsed = int.Parse(stringWithDigits);

            return digitsParsed;
        }
    }
}
