using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.DTO;
using Repositories.Interfaces;
using Domain;
using Domain.Enums;
using Services.Interface;
using Services.ViewModels.EmailModels;
using System.Security.Policy;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebAPI.Controllers
{
    [Route("api/v1/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly    IAccountService _accountService;
        private readonly IEmailService _emailService;

        public AccountsController(IAccountService accountService, IEmailService emailService)
        {
            _accountService = accountService;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(AccountSignupModel accountLogin, [FromQuery] RoleEnums role)
        {
            try
            {
                var data = await _accountService.ResigerAsync(accountLogin, role.ToString());
                if (data.Status)
                {
                    var confirmationLink = Url.Action(nameof(ConfirmEmail), "Accounts", new { email = accountLogin.Email, token = data.Message }, Request.Scheme);
                    var message = new Message(new string[] { data.Data.Email }, "Confirmation email link", confirmationLink!);
                    await _emailService.SendEmail(message);
                    data.Message = "Added sucessfully, please check your email <3";
                    return Ok(data);
                }

                return BadRequest(data);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(AccountLoginModel account)
        {
            try
            {
                var result = await _accountService.LoginAsync(account);
                if (result.Status)
                {
                    return Ok(result);
                }
                return Unauthorized(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAccountsAsync()
        {
            return Ok(await _accountService.GetAllAccounts());
        }

        [HttpGet("test-email")]
        public async Task<IActionResult> TestEmail()
        {
            var message = new Message(new string[]
            {
                "manhdung5289@gmail.com"
            },
             "Test", 
             "<h1> Wassup bro ! </h1>"
            );

            await _emailService.SendEmail(message);

            return Ok(new { status = "success", Message = " email sent" });
        }

        [HttpGet("confirm-email")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            var result = await _accountService.ConfirmEmail(email, token);
            if (result)
            {
                return Ok(new { status = true, message ="oh yeah lmao u did it"});
            }
            return BadRequest(new { status = false, message = "bruh we ded 💀" });
        }

        [HttpGet("forget-passwrord")]
        public async Task<IActionResult> ConfirmEmail(string email)
        {
            var result = await _accountService.ForgotPassword(email);
            if (result.Status)
            {
                var confirmationLink = "Code:\n\"" + result.Data+"\"";
                var message = new Message(new string[] { email }, "Reset password token", confirmationLink!);
                await _emailService.SendEmail(message);
                return Ok(result);
            }
            return NotFound(result);
        }

        [HttpPut("password-reset")]
        public async Task<IActionResult> ResetPassword(string token , string email, string newPassword) 
        {
            return Ok(await _accountService.AccountChangePasswordAsync(email,token,newPassword));
        }






    }
}
