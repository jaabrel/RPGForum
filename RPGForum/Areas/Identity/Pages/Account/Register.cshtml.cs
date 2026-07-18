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
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;
using RPGForum.Models;

namespace RPGForum.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<Utilizadores> _signInManager;
        private readonly UserManager<Utilizadores> _userManager;
        private readonly IUserStore<Utilizadores> _userStore;
        private readonly IUserEmailStore<Utilizadores> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<Utilizadores> userManager,
            IUserStore<Utilizadores> userStore,
            SignInManager<Utilizadores> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

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
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required(ErrorMessage = "O nome de utilizador é obrigatório")]
            [StringLength(30, ErrorMessage = "O nome de utilizador deve ter entre {2} e {1} caracteres.", MinimumLength = 3)]
            [Display(Name = "Nome de Utilizador")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "O email é obrigatório")]
            [EmailAddress(ErrorMessage = "Formato de email inválido")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "A senha é obrigatória")]
            [StringLength(100, ErrorMessage = "A senha deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Senha")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                // Verificar se o email já está em uso na tabela Utilizadores
                var emailExists = await _context.Utilizadores.AnyAsync(u => u.Email == Input.Email);
                if (emailExists)
                {
                    ModelState.AddModelError("Input.Email", "Este endereço de email já está registado.");
                    return Page();
                }

                // Verificar se o nome de utilizador já está em uso na tabela Utilizadores
                var usernameExists = await _context.Utilizadores.AnyAsync(u => u.UserName == Input.Username);
                if (usernameExists)
                {
                    ModelState.AddModelError("Input.Username", "Este nome de utilizador já está em uso.");
                    return Page();
                }

                // Verificar se o utilizador já existe no Identity mas não está confirmado
                var existingUser = await _userManager.FindByEmailAsync(Input.Email);
                if (existingUser != null)
                {
                    if (!existingUser.EmailConfirmed)
                    {
                        var code = await _userManager.GenerateUserTokenAsync(existingUser, "Email", "ConfirmEmail");
                        await _emailSender.SendEmailAsync(Input.Email, "Código de Validação - RPGForum",
                            $"Olá {Input.Username},<br/><br/>O teu código de confirmação de conta no RPGForum é: <strong>{code}</strong>");

                        return RedirectToPage("ConfirmCode", new { email = Input.Email, username = Input.Username, returnUrl = returnUrl });
                    }
                    else
                    {
                        ModelState.AddModelError("Input.Email", "Este endereço de email já está registado.");
                        return Page();
                    }
                }

                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Gerar código numérico de 6 dígitos
                    var code = await _userManager.GenerateUserTokenAsync(user, "Email", "ConfirmEmail");

                    // Enviar e-mail
                    await _emailSender.SendEmailAsync(Input.Email, "Código de Validação - RPGForum",
                        $"Olá {Input.Username},<br/><br/>O teu código de confirmação de conta no RPGForum é: <strong>{code}</strong>");

                    // Redirecionar para a página de introdução do código
                    return RedirectToPage("ConfirmCode", new { email = Input.Email, username = Input.Username, returnUrl = returnUrl });
                }
                foreach (var error in result.Errors)
                {
                    var description = error.Code switch
                    {
                        "PasswordRequiresNonAlphanumeric" => "A palavra-passe deve conter pelo menos um caráter especial (ex: !, @, #, etc.).",
                        "PasswordRequiresDigit" => "A palavra-passe deve conter pelo menos um número ('0'-'9').",
                        "PasswordRequiresLower" => "A palavra-passe deve conter pelo menos uma letra minúscula ('a'-'z').",
                        "PasswordRequiresUpper" => "A palavra-passe deve conter pelo menos uma letra maiúscula ('A'-'Z').",
                        "PasswordTooShort" => "A palavra-passe deve ter pelo menos 6 caracteres.",
                        "DuplicateUserName" => "Este nome de utilizador ou email já está em uso.",
                        "DuplicateEmail" => "Este endereço de email já está em uso.",
                        _ => error.Description
                    };
                    ModelState.AddModelError(string.Empty, description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private Utilizadores CreateUser()
        {
            try
            {
                return Activator.CreateInstance<Utilizadores>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(Utilizadores)}'. " +
                    $"Ensure that '{nameof(Utilizadores)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<Utilizadores> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<Utilizadores>)_userStore;
        }
    }
}
