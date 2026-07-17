using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RPGForum.Models;

namespace RPGForum.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {

        public DbSet<Utilizadores> Utilizadores { get; set; }
        public DbSet<Personagens> Personagens { get; set; }
        public DbSet<Build> Builds { get; set; }
        public DbSet<Estatisticas> Estatisticas { get; set; }
        public DbSet<Armas> Armas { get; set; }
        public DbSet<Acessorios> Acessorios { get; set; }
        public DbSet<Comment> Comentario { get; set; }
        public DbSet<Like> Gostos { get; set; }
        public DbSet<BuildWeapon> BuildWeapons { get; set; }
        public DbSet<BuildAccessory> BuildAccessories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "a", Name = "Administrator", NormalizedName = "ADMINISTRADOR", ConcurrencyStamp = "991937ec-0e3f-45a7-adc0-6097a2c7f2bc" });

            // Atualizado para usar Utilizadores
            builder.Entity<Utilizadores>().HasData(
                new Utilizadores
                {
                    Id = "admin",
                    UserName = "admin@mail.pt",
                    NormalizedUserName = "ADMIN@MAIL.PT",
                    Email = "admin@mail.pt",
                    NormalizedEmail = "ADMIN@MAIL.PT",
                    EmailConfirmed = true,
                    SecurityStamp = "1bcbd0a7-5c9d-4510-811a-cd5eee6c0dbe",
                    ConcurrencyStamp = "fe626d10-17f8-4768-8818-895627758300",
                    PasswordHash = "AQAAAAEAACcQAAAAEHX9OhBfKGUsbLTayhBCZ3WRcp+X+ivBA00sBQI5YG2NPaVbTJsVKE9jOgm/4Sb2RQ==",
                    Role = "Administrator",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = "admin", RoleId = "a" });
            builder.Entity<BuildWeapon>().HasKey(bw => new { bw.BuildId, bw.WeaponId });
            builder.Entity<BuildAccessory>().HasKey(ba => new { ba.BuildId, ba.AccessoryId });
            builder.Entity<Like>().HasIndex(l => new { l.BuildId, l.UserId }).IsUnique();
            
            builder.Entity<Estatisticas>()
                .HasOne(e => e.Build).WithOne(b => b.Stats).HasForeignKey<Estatisticas>(e => e.BuildId);
            
            builder.Entity<Comment>()
                .HasOne(c => c.Parent).WithMany(c => c.Replies).HasForeignKey(c => c.ParentId).OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<Build>()
                .HasMany(b => b.Comments).WithOne(c => c.Build).OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<Build>()
                .HasMany(b => b.Likes).WithOne(L => L.Build).OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<Comment>()
                .HasOne(c => c.User).WithMany(u => u.Comments).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<Like>()
                .HasOne(l => l.User).WithMany(u => u.Likes).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Restrict);

        }


    }
}
