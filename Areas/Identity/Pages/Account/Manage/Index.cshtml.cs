
using System.ComponentModel.DataAnnotations;
using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using To_Do_List.Areas.UserArea.Data;

namespace To_Do_List.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IEmailSender  _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public IndexModel(
			 AppDbContext context,
			IEmailSender emailSender,
			UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _context= context;
            _emailSender = emailSender;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            /// 

            [Required(ErrorMessage ="User Name Required")]
            
			public String UserName { get; set; }

            [Required(ErrorMessage ="Email Required")]
            [EmailAddress]
			public string Email { get; set; }

		}

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var UserEmail= await _userManager.GetEmailAsync(user);  

           

            Input = new InputModel
            {
                Email = UserEmail,
                UserName = userName

			};
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var Username = await _userManager.GetUserNameAsync(user);
            if (Input.UserName != Username)
            {
                var SetUsername = await _userManager.SetUserNameAsync(user, Input.UserName);
                if (!SetUsername.Succeeded)
                {
                    StatusMessage = "Invalid User Name.";
                    return RedirectToPage();
                }
            }
			var UserEmail = await _userManager.GetEmailAsync(user);
            if (Input.Email != UserEmail) 
            {
                try 
                {

				    var setEmail = await _userManager.SetEmailAsync(user, Input.Email);
				    if (!setEmail.Succeeded)
				    {
					StatusMessage = "User Different Email Address";
					return RedirectToPage();
			    	}
                else
				{
					try
					{
						StringBuilder builder = new StringBuilder();
						Random random = new Random();

						for (int i = 0; i < 5; i++)
						{
							builder.Append(random.Next(1, 10));
						}
						await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
							$"Validation Number:{builder.ToString()}");

						_context.Entry<IdentityUser>(user).Property("ConfirmationCode").CurrentValue = builder.ToString();
						_context.SaveChanges();
					}
					catch (Exception e)
					{

						ModelState.AddModelError(string.Empty, e.Message);
						return RedirectToPage();

					    }
                    }

				}
                catch
                {
					StatusMessage = "User Different Email Address";
					return RedirectToPage();
				}

			}

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
