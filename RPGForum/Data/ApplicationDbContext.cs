using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RPGForum.Models;

namespace RPGForum.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Utilizadores> Utilizadores { get; set; }
        public DbSet<Personagens> Personagens { get; set; }
        public DbSet<BuildPost> Builds { get; set; }
        public DbSet<Estatísticas> Estatisticas { get; set; }
        public DbSet<Armas> Armas { get; set; }
        public DbSet<Acessórios> Acessorios { get; set; }
        public DbSet<Comment> Comentario { get; set; }
        public DbSet<Like> Gostos { get; set; }
        public DbSet<BuildWeapon> BuildWeapons { get; set; }
        public DbSet<BuildAccessory> BuildAccessories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
     
        }
    
        
    }
}
