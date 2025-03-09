using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewsManagementSystem.BusinessLogic.Services;
using NewsManagementSystem.DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsManagementSystem.Pages.Admin
{
    public class UserManagementModel : PageModel
    {
        private readonly AccountService _accountService;

        public UserManagementModel(AccountService accountService)
        {
            _accountService = accountService;
        }

        [BindProperty]
        public SystemAccount Account { get; set; }

        public IEnumerable<SystemAccount> Accounts { get; set; }

        public async Task OnGetAsync()
        {
            Accounts = await _accountService.GetAllAccont();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Account.AccountId = await _accountService.GetNextId();
            await _accountService.AddAccount(Account);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _accountService.UpdateAccount(Account);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(short id)
        {
            await _accountService.DeleteAccount(id);
            return RedirectToPage();
        }
    }
}
