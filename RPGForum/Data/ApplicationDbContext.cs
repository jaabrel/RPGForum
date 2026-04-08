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
        public DbSet<Estatisticas> Estatisticas { get; set; }
        public DbSet<Armas> Armas { get; set; }
        public DbSet<Acessorios> Acessorios { get; set; }
        public DbSet<Comment> Comentario { get; set; }
        public DbSet<Like> Gostos { get; set; }
        public DbSet<BuildWeapon> BuildWeapons { get; set; }
        public DbSet<BuildAccessory> BuildAccessories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BuildWeapon>()
                .HasKey(bw => new { bw.BuildId, bw.WeaponId });
            modelBuilder.Entity<BuildAccessory>()
                .HasKey(ba => new { ba.BuildId, ba.AccessoryId });

            modelBuilder.Entity<Like>()
                .HasIndex(l => new { l.BuildId, l.UserId }).IsUnique();
            modelBuilder.Entity<Utilizadores>()
                .HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Utilizadores>()
                .HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<Estatisticas>()
                .HasOne(e => e.Build)
                .WithOne(b => b.Stats)
                .HasForeignKey<Estatisticas>(e => e.BuildId);
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Parent)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<BuildPost>()
                .HasMany(b => b.Comments)
                .WithOne(c => c.Build)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<BuildPost>()
                .HasMany(b => b.Likes)
                .WithOne(L => L.Build)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);
                
        }
    
        
    }
}
