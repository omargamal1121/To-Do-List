
#nullable disable


using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace To_Do_List.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, ILogger<LoginModel> logger)
        {

            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }


        public IList<AuthenticationScheme> ExternalLogins { get; set; }





        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
 
            [Required]
            public string username { get; set; }


            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }


        }

        public async Task OnGetAsync()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            
            if (ModelState.IsValid)
            {
				var result = await _signInManager.PasswordSignInAsync(Input.username, Input.Password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return RedirectToAction("Index", "User", new{ area="UserArea" });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "UserName or password invalid");
                    return Page();
                }
            }
            return Page();
        }
		
		
		}
	
}
