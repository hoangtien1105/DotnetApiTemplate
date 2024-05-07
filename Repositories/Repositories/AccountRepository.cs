using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repositories.Commons;
using Repositories.DTO;
using Repositories.Entities;
using Repositories.Interfaces;
using Repositories.Utils;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly TemplateDbContext _templateDbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        private readonly IConfiguration _configuration;
        // identity collection
        private readonly UserManager<Account> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<Account> _signInManager;

        public AccountRepository(TemplateDbContext templateDbContext, ICurrentTime timeService, IClaimsService claimsService, UserManager<Account> userManager, RoleManager<IdentityRole> roleManager, SignInManager<Account> signInManager, IConfiguration configuration)
        {
            _templateDbContext = templateDbContext;
            _timeService = timeService;
            _claimsService = claimsService;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<List<string>> GetRoleName(Account account)
        {
            var result = await _userManager.GetRolesAsync(account);
            return result.ToList();
        }

        public async Task<Account> AddAccount(AccountSignupModel account, string role)
        {
            try
            {
                var userExist = await _userManager.FindByEmailAsync(account.Email);
                if (userExist != null)
                {
                    return null;
                }

                var user = new Account
                {
                    Email = account.Email,
                    UserName = account.Email,
                    FullName = account.FullName,
                    Dob = account.Dob,
                    Gender = account.Gender.ToLower() == "male"? true:false,
                    CreatedBy = _claimsService.GetCurrentUserId,
                    CreatedDate = _timeService.GetCurrentTime()
                };

                if (account.FullName != null)
                {
                    user.UnsignFullName = StringTools.ConvertToUnSign(account.FullName);
                }

                var result = await _userManager.CreateAsync(user, account.Password);

                if (result.Succeeded)
                {
                    Console.WriteLine($"New user ID: {user.Id}");
                    if (!await _roleManager.RoleExistsAsync(role.ToString()))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(role.ToString()));
                    }

                    if (await _roleManager.RoleExistsAsync(role.ToString()))
                    {
                        await _userManager.AddToRoleAsync(user, role.ToString());
                    }

                    if (!await _roleManager.RoleExistsAsync(role.ToString()))
                        await _roleManager.CreateAsync(new IdentityRole(role));

                    if (await _roleManager.RoleExistsAsync(role.ToString()))
                    {
                        await _userManager.AddToRoleAsync(user, role.ToString());
                    }
                    return user;
                }
                else
                {
                    // Tạo người dùng không thành công, xem thông tin lỗi và xử lý
                    StringBuilder errorValue = new StringBuilder();
                    foreach (var item in result.Errors)
                    {
                        errorValue.Append($"{item.Description}");
                    }
                    throw new Exception(errorValue.ToString()); // bắn zề cho GlobalEx midw

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> GenerateEmailConfirmationToken(Account user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<string> GenerateTokenForResetPassword(Account user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }
        public async Task<ResponseLoginModel> LoginByEmailAndPassword(AccountLoginModel account)
        {
            var accountExist = await _userManager.FindByEmailAsync(account.Email);
            if(accountExist == null)
            {
                return null;
            }

            var result = await _signInManager.PasswordSignInAsync(account.Email, account.Password, false, false);

            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(accountExist);

                var authClaims = new List<Claim> // add account vào claim
                {
                    new Claim("UserId", accountExist.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                foreach (var role in roles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }
                //generate refresh token
                var refreshToken = TokenTools.GenerateRefreshToken();
                _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);
                accountExist.RefreshToken = refreshToken;
                accountExist.RefreshTokenExpiryTime = _timeService.GetCurrentTime().AddDays(refreshTokenValidityInDays);

                await _userManager.UpdateAsync(accountExist); //update 2 jwt
                var token = GenerateJWTToken.CreateToken(authClaims, _configuration, _timeService.GetCurrentTime());
                return new ResponseLoginModel
                {
                    Status = true,
                    Message = "Login successfully",
                    JWT = new JwtSecurityTokenHandler().WriteToken(token),
                    Expired = token.ValidTo,
                    JWTRefreshToken = refreshToken,
                };
            }
            else
            {
                if (!accountExist.EmailConfirmed)
                {
                    return new ResponseLoginModel
                    {
                        Status = false,
                        Message = "Your email haven't verified yet, please check",
                    };
                }

                return new ResponseLoginModel
                {
                    Status = false,
                    Message = "Incorrect email or password",
                };
            }

        }

        public async Task<Account> GetAccountByEmailAsync(string email)
        {
            var result = await _userManager.FindByEmailAsync(email);
            if (result == null)
            {
                return null;
            }
            return result;

        }

        public async Task<List<Account>> GetAllAccountsAsync()
        {
            try
            {
                // get all users
                var accounts = await _userManager.Users.ToListAsync();
                return accounts;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<AccountDetailsModel>> GetAllAccountsWithRoleAsync()
        {
            // Bước 1: Lấy danh sách người dùng
            var users = _userManager.Users;

            // Bước 2: Kết hợp thông tin người dùng, vai trò và lấy danh sách AccountDetailsModel ban đầu
            var accountDetailsModels = await (from user in users
                                              join userRole in _templateDbContext.UserRoles on user.Id equals userRole.UserId
                                              join role in _templateDbContext.Roles on userRole.RoleId equals role.Id
                                              group new {user, role} by user.Id into userRolesGroup
                                              select new AccountDetailsModel
                                              {
                                                  Id = userRolesGroup.Key,
                                                  UnsignFullName = userRolesGroup.First().user.UnsignFullName,
                                                  FullName = userRolesGroup.First().user.FullName,
                                                  Dob = userRolesGroup.First().user.Dob,
                                                  Gender = (bool)userRolesGroup.First().user.Gender ? "male" :"female",
                                                  Image = userRolesGroup.First().user.Image,
                                                  IsDeleted = userRolesGroup.First().user.IsDeleted,
                                                  Role = userRolesGroup.Select( urg => new RoleInfoModel
                                                  {
                                                      RoleId = urg.role.Id,
                                                      RoleName = urg.role.Name
                                                  }).ToList()
                                              }).ToListAsync();

            return accountDetailsModels;

        }

        public async Task<Account> ChangeAccountPasswordAsync(string email, string token, string newPassword)
        {
            try
            {
                var account = await _userManager.FindByEmailAsync(email);
                if (account == null)
                {
                    return null;
                }

                var changePasswordResult = await _userManager.ResetPasswordAsync(account, token, newPassword);
                if (changePasswordResult.Succeeded)
                {
                    _templateDbContext.Update(account);
                    return account;
                }
                else
                {
                    var errorMessage = string.Join(", ", changePasswordResult.Errors.Select(e => e.Description));
                    throw new Exception($"Error changing password: {errorMessage}");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> ConfirmEmail ( string email , string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    //_templateDbContext.Update(user);
                    //await _templateDbContext.SaveChangesAsync();
                    return true;
                }
                else
                {
                    // Verify người dùng không thành công, xem thông tin lỗi và xử lý
                    StringBuilder errorValue = new StringBuilder();
                    foreach (var item in result.Errors)
                    {
                        errorValue.Append($"{item.Description}");
                    }
                    throw new Exception(errorValue.ToString()); // bắn zề cho GlobalEx midw
                }
            }
            return false;
        }

        public async Task<Pagination<Account>> GetAccountsByFiltersAsync(PaginationParameter paginationParameter, AccountFilterModel accountFilterModel)
        {
            var accountsQuery = _templateDbContext.Accounts.AsQueryable();
            accountsQuery = await ApplyFilterSortAndSearch(accountsQuery, accountFilterModel);
            if (accountsQuery != null)
            {
                var sortedQuery = ApplySorting(accountsQuery, accountFilterModel);
                var totalCount = await sortedQuery.CountAsync();
                var accountsPagination = await sortedQuery
                    .Skip((paginationParameter.PageIndex - 1) * paginationParameter.PageSize)
                    .Take(paginationParameter.PageSize)
                    .ToListAsync();
                return new Pagination<Account>(accountsPagination, totalCount, paginationParameter.PageIndex, paginationParameter.PageSize);
            }
            return null;
        }



        private IQueryable<Account> ApplySorting (IQueryable<Account> query , AccountFilterModel accountFilterModel)
        {
            switch (accountFilterModel.Sort.ToLower())
            {
                case "fullname":
                    query = (accountFilterModel.SortDirection.ToLower() == "asc") ? query.OrderBy(a => a.FullName) : query.OrderByDescending(a => a.FullName);
                    break;
                case "dob":
                    query = (accountFilterModel.SortDirection.ToLower() == "asc") ? query.OrderBy(a => a.Dob) : query.OrderByDescending(a => a.Dob);
                    break;
                default:
                    query = (accountFilterModel.SortDirection.ToLower() == "asc") ? query.OrderBy(a => a.Id) : query.OrderByDescending(a => a.Id);
                    break;
            }

            return query;
        }

        private async Task<IQueryable<Account>> ApplyFilterSortAndSearch(IQueryable<Account> query, AccountFilterModel accountFilterModel)
        {
            if (accountFilterModel == null)
            {
                return query;
            }

            if (accountFilterModel.isDeleted == true)
            {
                query = query.Where(a => a.IsDeleted == true);
            }
            else if (accountFilterModel.isDeleted == false)
            {
                query = query.Where(a => a.IsDeleted == false);
            }
            else
            {
                query = query.Where(a => a.IsDeleted == true || a.IsDeleted == false);
            }

            if (!string.IsNullOrEmpty(accountFilterModel.Gender))
            {
                bool isMale = accountFilterModel.Gender.ToLower() == "male";
                query = query.Where(a => a.Gender == isMale);
            }

            if (!string.IsNullOrEmpty(accountFilterModel.Role))
            {
                var accountsInRole = await _userManager.GetUsersInRoleAsync(accountFilterModel.Role);

                if (accountsInRole != null)
                {
                    var userIdsInRole = accountsInRole.Select(u => u.Id);
                    query = query.Where(a => userIdsInRole.Contains(a.Id));
                }
                else
                {
                    return null;
                }
            }

            if (!string.IsNullOrEmpty(accountFilterModel.Search))
            {
                query = query.Where(a =>
                    a.FullName.Contains(accountFilterModel.Search) ||
                    a.UnsignFullName.Contains(accountFilterModel.Search)
                );
            }
            return query;



        }

       

    
    }
}
