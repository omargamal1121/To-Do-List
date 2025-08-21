using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace To_Do_List.Models
{
	public class EmailModel : IEmailSender
	{
		public readonly EmailSettings _Settings;
		public EmailModel(IConfiguration _configuration)
		{
			_Settings = _configuration.GetSection("EmailSetting").Get<EmailSettings>();

		}
		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{

			using (MailMessage message = new MailMessage())
			{
				message.IsBodyHtml = true;
				message.From = new MailAddress(_Settings.Sender);
				message.Subject = subject;
				message.Body = $"<html>" +
				$"<body>" +
				$"{htmlMessage}" +
				$"</body>" +
				$"</html>";
				message.To.Add(new MailAddress(email));
				using (SmtpClient client = new SmtpClient(_Settings.Server))
				{
					client.EnableSsl = _Settings.Ssl;
					client.Credentials = new NetworkCredential(_Settings.Sender, _Settings.Pass);
					client.Port = _Settings.Port;
					client.UseDefaultCredentials = false;
					await client.SendMailAsync(message);
				}

			};
		}
	}
}
