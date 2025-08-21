// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using To_Do_List.Areas.UserArea.Data;

namespace To_Do_List.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IEmailSender _emailSender;

        public ResendEmailConfirmationModel(AppDbContext context, UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

    
        public class InputModel
        {
         
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email not found");
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            
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

			ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
            return RedirectToPage("ConfirmationPage", new { email =Input.Email});
        }
    }
}
