using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewsManagementSystem.BusinessLogic.Services;
using NewsManagementSystem.Models;
using System.Threading.Tasks;

namespace NewsManagementSystem.Pages.Authentication
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly AccountService _accountService;

        public IndexModel(IConfiguration configuration, AccountService accountService)
        {
            _configuration = configuration;
            _accountService = accountService;
        }

        [BindProperty]
        public LoginViewModel Login { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var adminEmail = _configuration["AdminAccount:Email"];
            var adminPassword = _configuration["AdminAccount:Password"];

            if (Login.Username == adminEmail && Login.Password == adminPassword)
            {
               
                return RedirectToPage("/Admin/NewsReport");
            }

          
            var userId = await _accountService.Login(Login.Username, Login.Password);
            if (userId == -1)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            var user = await _accountService.GetAccount(userId);
            if (user != null)
            {
               
                HttpContext.Session.SetInt32("AccountId", user.AccountId);

                if (user.AccountRole == 2) 
                {
                    return RedirectToPage("/Lecturer/Index");
                }
                else if (user.AccountRole == 1) 
                {
                    return RedirectToPage("/Staff/Index", new { id = userId });
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }
}