using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RPGForum.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;

namespace RPGForum.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<Utilizadores> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<Utilizadores> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();
        public bool EmailSent { get; set; } = false;
        public class InputModel
        {
            [Required(ErrorMessage = "O email é obrigatório.")]
            [EmailAddress(ErrorMessage = "Formato de email inválido.")]
            public string Email { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var user = await _userManager.FindByEmailAsync(Input.Email);
            // Por segurança, não revelamos se o email existe ou não
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                EmailSent = true;
                return Page();
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", token, email = Input.Email },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                Input.Email,
                "Recuperação de Palavra-passe - RPGForum",
                $"Para redefinires a tua palavra-passe, <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clica aqui</a>.");
            EmailSent = true;
            return Page();
        }
    }
}
