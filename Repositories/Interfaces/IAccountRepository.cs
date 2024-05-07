using Repositories.DTO;
using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account> AddAccount(AccountSignupModel account, String role);
        Task<Account> ChangeAccountPasswordAsync(string id, string currentPassword, string newPassword);
        Task<bool> ConfirmEmail(string email, string token);
        Task<string> GenerateEmailConfirmationToken(Account user);
        Task<string> GenerateTokenForResetPassword(Account user);
        Task<Account> GetAccountByEmailAsync(string email);
        Task<List<Account>> GetAllAccountsAsync();
        Task<List<AccountDetailsModel>> GetAllAccountsWithRoleAsync();
        Task<List<string>> GetRoleName(Account account);
        Task<ResponseLoginModel> LoginByEmailAndPassword(AccountLoginModel account);
    }
}
