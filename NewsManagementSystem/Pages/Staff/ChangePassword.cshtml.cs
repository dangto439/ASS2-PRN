using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewsManagementSystem.BusinessLogic.Services;
using System.Threading.Tasks;

namespace NewsManagementSystem.Pages.Staff
{
    public class ChangePasswordModel : PageModel
    {
        private readonly AccountService _accountService;

        public ChangePasswordModel(AccountService accountService)
        {
            _accountService = accountService;
        }

        [BindProperty(SupportsGet = true)]
        public short Id { get; set; }

        [BindProperty]
        public string OldPassword { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        public short AccountId => Id;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var account = await _accountService.GetAccount(Id);
            if (account == null || account.AccountPassword != OldPassword)
            {
                ModelState.AddModelError(string.Empty, "Incorrect old password.");
                return Page();
            }

            account.AccountPassword = NewPassword;
            await _accountService.UpdateAccount(account);

            return RedirectToPage("./Index", new { id = Id });
        }
    }
}
