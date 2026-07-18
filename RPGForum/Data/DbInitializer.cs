using Microsoft.EntityFrameworkCore;
using RPGForum.Models;

namespace RPGForum.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            var existing = await context.Personagens.ToListAsync();

            var targetCharacters = new List<Personagens>
            {
                new() 
                { 
                    Name = "Geralt de Rívia", 
                    Description = "Um caçador de monstros mutante lendário da Escola do Lobo, mestre em esgrima e sinais mágicos simples.", 
                    ImageUrl = "/images/characters/geralt.jpg" 
                },
                new() 
                { 
                    Name = "Yennefer de Vengerberg", 
                    Description = "Uma poderosa e inteligente feiticeira de Vengerberg, membro do Conselho e amante dos mistérios da magia.", 
                    ImageUrl = "/images/characters/yennefer.jpg" 
                },
                new() 
                { 
                    Name = "Legolas Greenleaf", 
                    Description = "Um elfo da floresta de Mirkwood, mestre arqueiro com visão aguçada, agilidade inigualável e audição apurada.", 
                    ImageUrl = "/images/characters/legolas.jpg" 
                },
                new() 
                { 
                    Name = "Ezio Auditore", 
                    Description = "Um nobre florentino que se tornou um Mentor lendário da Irmandade dos Assassinos, mestre da furtividade e parkour.", 
                    ImageUrl = "/images/characters/ezio.jpg" 
                }
            };

            // Se existirem registos, vamos atualizar os seus nomes e descrições para manter os IDs consistentes com builds existentes
            if (existing.Count > 0)
            {
                for (int i = 0; i < Math.Min(existing.Count, targetCharacters.Count); i++)
                {
                    existing[i].Name = targetCharacters[i].Name;
                    existing[i].Description = targetCharacters[i].Description;
                    existing[i].ImageUrl = targetCharacters[i].ImageUrl;
                }
                
                if (existing.Count < targetCharacters.Count)
                {
                    for (int i = existing.Count; i < targetCharacters.Count; i++)
                    {
                        context.Personagens.Add(targetCharacters[i]);
                    }
                }
            }
            else
            {
                await context.Personagens.AddRangeAsync(targetCharacters);
            }

            await context.SaveChangesAsync();
        }
    }
}
