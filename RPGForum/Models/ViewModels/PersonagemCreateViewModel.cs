using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace RPGForum.Models.ViewModels
{
    public class PersonagemCreateViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        // IFormFile é a interface do .NET para receber ficheiros no formulário
        public IFormFile? ImagemUpload { get; set; }
    }
}
