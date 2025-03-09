using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewsManagementSystem.BusinessLogic.Services;
using NewsManagementSystem.DataAccess.Models;

namespace NewsManagementSystem.Pages.Staff
{
    public class IndexModel : PageModel
    {
        private readonly AccountService _accountService;

        public IndexModel(AccountService accountService)
        {
            _accountService = accountService;
        }

        [BindProperty]
        public SystemAccount Account { get; set; }

        public async Task<IActionResult> OnGetAsync(short id)
        {
            Account = await _accountService.GetAccount(id);
            if (Account == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _accountService.UpdateAccount(Account);

            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToPage(new { id = Account.AccountId });
        }
    }
}
