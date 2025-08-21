
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text;

using To_Do_List.Areas.UserArea.Data;

namespace To_Do_List.Areas.Identity.Pages.Account
{
    public class ConfirmationPageModel : PageModel
    {
		private readonly IEmailSender _emailSender;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly AppDbContext _context;
		public  string _email { get; set; }
		public ConfirmationPageModel(IEmailSender emailSender,AppDbContext context,UserManager<IdentityUser>userManager)
		{
			_userManager = userManager;
			_context=context;
			_emailSender = emailSender;

			
		}

		[BindProperty]
		public InputModel Input { get; set; }
        public void OnGet(string email)
        {
			if (string.IsNullOrEmpty(email))
			{
				RedirectToPage("/Error"); 
			}
			_email = email;
        }
        public async Task<IActionResult> OnPost(string email)
        {
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null) 
			{
				return RedirectToPage("/Error");
			}
			var confirmationCode = _context.Entry(user).Property("ConfirmationCode").CurrentValue?.ToString();
			if (confirmationCode == Input.Validation_Number)
			{
				user.EmailConfirmed = true;
			 await	_userManager.UpdateAsync(user);
				return RedirectToAction("Index", "user", new {Area="UserArea"});
			}
			else 
            {

                ModelState.AddModelError("Validation Number", "Wrong Namber");
                ModelState.AddModelError("Validation Number", "Check Ur Email");
				StringBuilder builder = new StringBuilder();
				Random random = new Random();

				for (int i = 0; i < 5; i++)
				{
					builder.Append(random.Next(1, 10));
				}
				await _emailSender.SendEmailAsync(email, "Confirm your email",
					$"Validation Number:{builder.ToString()}");

				_context.Entry<IdentityUser>(user).Property("ConfirmationCode").CurrentValue = builder.ToString();
				_context.SaveChanges();
				return Page();
            }
        }
        public class InputModel 
        {
			[BindProperty]
			[Required(ErrorMessage = "Validation Number is Requierd")]

			public string Validation_Number { get; set; }
		}
    }
}
