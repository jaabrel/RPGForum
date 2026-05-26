using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RPGForum.Models;
using RPGForum.Data;
using Microsoft.AspNetCore.Authorization;

namespace RPGForum.Pages.PersonagensPages;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Personagens> Personagens { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Personagens = await _context.Personagens.ToListAsync();
    }
}
