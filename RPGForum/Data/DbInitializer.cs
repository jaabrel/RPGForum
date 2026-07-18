using Microsoft.EntityFrameworkCore;
using RPGForum.Models;

namespace RPGForum.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            try
            {
                using (var cmd = context.Database.GetDbConnection().CreateCommand())
                {
                    await context.Database.OpenConnectionAsync();

                    // 1. Rename initial migration from 20260714100833_DB1 to 20260717150801_InitialCreate
                    cmd.CommandText = "UPDATE __EFMigrationsHistory SET MigrationId = '20260717150801_InitialCreate' WHERE MigrationId = '20260714100833_DB1';";
                    await cmd.ExecuteNonQueryAsync();

                    // 2. Rename stats migration from 20260718174412_AddEquipStats to 20260718190342_AddEquipStats
                    cmd.CommandText = "UPDATE __EFMigrationsHistory SET MigrationId = '20260718190342_AddEquipStats' WHERE MigrationId = '20260718174412_AddEquipStats';";
                    await cmd.ExecuteNonQueryAsync();

                    // 3. Set journal mode to DELETE permanently
                    cmd.CommandText = "PRAGMA journal_mode=DELETE;";
                    await cmd.ExecuteNonQueryAsync();

                    // 4. Update seeded admin's UserName to 'admin' if it is still the email
                    cmd.CommandText = "UPDATE AspNetUsers SET UserName = 'admin', NormalizedUserName = 'ADMIN' WHERE Id = 'admin' AND UserName = 'admin@mail.pt';";
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB SYNC] Synchronization warning: {ex.Message}");
            }



            await context.Database.MigrateAsync();

            var existing = await context.Personagens.ToListAsync();

            var targetCharacters = new List<Personagens>
            {
                new() 
                { 
                    Name = "Ichigo Kurosaki", 
                    Description = "Um Shinigami substituto de Karakura, portador da icónica Zanpakutou Zangetsu e mestre do Getsuga Tenshou.", 
                    ImageUrl = "/images/ichigo.png" 
                },
                new() 
                { 
                    Name = "Goku", 
                    Description = "Um lendário guerreiro Saiyajin criado na Terra, mestre de artes marciais, famoso pelo seu Kamehameha e pela busca constante de superar os seus limites.", 
                    ImageUrl = "/images/goku.png" 
                },
                new() 
                { 
                    Name = "Sonic the Hedgehog", 
                    Description = "O ouriço azul mais rápido do mundo, defensor da liberdade que usa a sua velocidade supersónica e spin dash para derrotar o Dr. Eggman.", 
                    ImageUrl = "/images/sonic.png" 
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
                else if (existing.Count > targetCharacters.Count)
                {
                    var idsToRemove = existing.Skip(targetCharacters.Count).Select(e => e.Id).ToList();
                    var buildsToUpdate = await context.Builds.Where(b => idsToRemove.Contains(b.CharacterId)).ToListAsync();
                    foreach (var build in buildsToUpdate)
                    {
                        build.CharacterId = existing[0].Id;
                    }

                    for (int i = targetCharacters.Count; i < existing.Count; i++)
                    {
                        context.Personagens.Remove(existing[i]);
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
