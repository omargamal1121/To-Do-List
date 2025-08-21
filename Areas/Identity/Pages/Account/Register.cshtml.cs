// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using To_Do_List.Areas.UserArea.Data;

namespace To_Do_List.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            AppDbContext context,
            UserManager<IdentityUser> userManager,
          
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
           
        }


        [BindProperty]
        public InputModel Input { get; set; }

      

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

    
        public class InputModel
        {
         
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }



            [Required(ErrorMessage ="Name is Required")]
            [Display(Name = "UserName")]
            [StringLength(100,MinimumLength = 5,ErrorMessage = "Must be greater than 5\"")]
			public string UserName { get; set; }

	
			[Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

           
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync()
        {
           
           
        }

        public async Task<IActionResult> OnPostAsync()
        {
 
         
            if((await _userManager.FindByEmailAsync(Input.Email)) is not null )
            {
                ModelState.AddModelError("", "Email is used before");
                if(!(await _userManager.FindByEmailAsync(Input.Email)).EmailConfirmed)
                ModelState.AddModelError("", "Use differente email or go to resend email");
                return Page();
			}
			if (ModelState.IsValid)
            {
                var user = CreateUser();

                user.Email = Input.Email;
                user.UserName= Input.UserName;
               
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                  
                    await SendEmail(user);


					return RedirectToPage("ConfirmationPage", new { email = Input.Email });

				}
               

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
     

            
            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

      
        public async Task  SendEmail(IdentityUser user)
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
                RedirectToPage("Register");
                
            }


		}
	}
}
