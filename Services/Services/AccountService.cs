using AutoMapper;
using Repositories.DTO;
using Repositories.Interfaces;
using Services.Interface;
using Services.ViewModels.AccountModels;
using Services.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<AccountDetailsModel>> GetAllAccounts()
        {
            //var accounts = await _unitOfWork.AccountRepository.GetAllAccountsAsync();
            //var result = new List<AccountDetailsModel>();
            //foreach (var account in accounts)
            //{
            //    var roleName = await _unitOfWork.AccountRepository.GetRoleName(account);
            //    var lmao = _mapper.Map<AccountDetailsModel>(account);
            //    lmao.RoleName = roleName;
            //    result.Add(lmao);
            //}

            var result = await _unitOfWork.AccountRepository.GetAllAccountsWithRoleAsync();

            return result;
        }

        public Task<AccountDetailsModel> GetAccountByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseGenericModel<AccountDetailsModel>> ResigerAsync(AccountSignupModel accountLogin, string role)
        {
            var result = await _unitOfWork.AccountRepository.AddAccount(accountLogin, role);
            if (result == null)
            {
                return new ResponseGenericModel<AccountDetailsModel>
                {
                    Data = null,
                    Status = false,
                    Message = "Add account has been failed"
                };
            }

            var token = await _unitOfWork.AccountRepository.GenerateEmailConfirmationToken(result);

            return new ResponseGenericModel<AccountDetailsModel>
            {
                Data = _mapper.Map<AccountDetailsModel>(result),
                Status = true,
                Message = token
            };
        }

        public async Task<ResponseLoginModel> LoginAsync(AccountLoginModel account)
        {
            return await _unitOfWork.AccountRepository.LoginByEmailAndPassword(account);
        }

        public async Task<ResponseGenericModel<AccountDetailsModel>> AccountChangePasswordAsync(string email, string token, string newPassword)
        {
            var result = await _unitOfWork.AccountRepository.ChangeAccountPasswordAsync(email, token, newPassword);
            if ( result == null)
            {
                return new ResponseGenericModel<AccountDetailsModel> { Data = null, Status = false, Message = "The update process has been cooked, pleas try again" };
            }

            return new ResponseGenericModel<AccountDetailsModel>
            {
                Data = _mapper.Map<AccountDetailsModel>(result),
                Status = true,
                Message = "Update Sucessfully"
            };

        }

        public async Task<ResponseGenericModel<string>> ForgotPassword(string email)
        {
            var user = await _unitOfWork.AccountRepository.GetAccountByEmailAsync(email);
            if (user == null)
            {
                return new ResponseGenericModel<string> { Data = null, Status = false, Message = "Account is not existed" };

            }
            else
            {
                return new ResponseGenericModel<string>
                {
                    Data = await _unitOfWork.AccountRepository.GenerateTokenForResetPassword(user),
                    Status = true,
                    Message = "Token to your email " + user.Email + " have been sent for reset password"
                };
            }

        }
        public async Task<bool> ConfirmEmail ( string email, string token)
        {
            return await _unitOfWork.AccountRepository.ConfirmEmail(email, token);
        }




    }
}
