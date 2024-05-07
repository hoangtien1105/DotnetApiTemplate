using Repositories.DTO;
using Services.ViewModels.AccountModels;
using Services.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IAccountService
    {
        Task<ResponseGenericModel<AccountDetailsModel>> ResigerAsync(AccountSignupModel accountLogin ,string role);
        Task<AccountDetailsModel> GetAccountByEmail(string email);
        Task<ResponseLoginModel> LoginAsync(AccountLoginModel account);
        Task<List<AccountDetailsModel>> GetAllAccounts();
        Task<ResponseGenericModel<AccountDetailsModel>> AccountChangePasswordAsync(string email, string currentPassword, string newPassword);
        Task<bool> ConfirmEmail(string email, string token);
        Task<ResponseGenericModel<string>> ForgotPassword(string email);
    }
}
