using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;
using RPGForum.Models;
namespace RPGForum.Pages.Admin.Acessorios;

[Authorize(Roles = "Administrator")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Models.Acessorios> Acessorios { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Acessorios = await _context.Acessorios.ToListAsync();
    }
}