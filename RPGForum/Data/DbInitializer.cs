using Microsoft.EntityFrameworkCore;
using RPGForum.Models;

namespace RPGForum.Data
{

    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Garante que a base de dados existe e as migrações estão aplicadas
            await context.Database.MigrateAsync();
            // 1. POPULAR PERSONAGENS (se a tabela estiver vazia)
            if (!await context.Personagens.AnyAsync())
            {
                var personagens = new List<Personagens>
                {
                    new()
                    {
                        Name = "Ichigo Kurosaki",
                        Description = "Shinigami substituto de Karakura, portador da Zanpakutou Zangetsu e mestre do Getsuga Tenshou.",
                        
                    },
                    new()
                    {
                        Name = "Goku",
                        Description = "Guerreiro Saiyajin criado na Terra, mestre em artes marciais e mestre do icónico Kamehameha.",
                        
                    },
                    new()
                    {
                        Name = "Sonic the Hedgehog",
                        Description = "O ouriço azul mais rápido do mundo, defensor da liberdade através da sua velocidade supersónica.",
                        
                    }
                };
                await context.Personagens.AddRangeAsync(personagens);
                await context.SaveChangesAsync();
            }
            // 2. POPULAR ARMAS (se a tabela estiver vazia)
            if (!await context.Armas.AnyAsync())
            {
                var armas = new List<Armas>
                {
                    new() { Name = "Zangetsu (Bankai)", Type = "Espada", Description = "Lâmina escura de shinigami que amplia o poder espiritual.", StatAfetada = "Força", StatBonus = "+45 Força" },
                    new() { Name = "Báculo Mágico (Nyoibo)", Type = "Cajado", Description = "Bastão que estica até ao infinito conforme a vontade do lutador.", StatAfetada = "Defesa", StatBonus = "+30 Defesa" },
                    new() { Name = "Luvas Supersónicas", Type = "Manoplas", Description = "Manoplas que canalizam o atrito do vento em golpes explosivos.", StatAfetada = "Velocidade", StatBonus = "+50 Velocidade" },
                    new() { Name = "Arco de Energia Espiritual", Type = "Arco", Description = "Dispara setas criadas a partir do poder mágico puro.", StatAfetada = "Magia", StatBonus = "+40 Magia" }
                };
                await context.Armas.AddRangeAsync(armas);
                await context.SaveChangesAsync();
            }
            // 3. POPULAR ACESSÓRIOS (se a tabela estiver vazia)
            if (!await context.Acessorios.AnyAsync())
            {
                var acessorios = new List<Acessorios>
                {
                    new() { Name = "Brinco Potara", Type = "Brinco", Description = "Jóia mágica que funde e multiplica exponencialmente o poder interior.", StatAfetada = "Força", StatBonus = "+25 Todos os Atributos" },
                    new() { Name = "Botas de Alta Fricção", Type = "Calçado", Description = "Calçado resistente concebido para corridas a velocidade da luz.", StatAfetada = "Velocidade", StatBonus = "+35 Velocidade" },
                    new() { Name = "Máscara Hollow", Type = "Máscara", Description = "Concede um aumento monstruoso de poder espiritual e regeneração.", StatAfetada = "HP", StatBonus = "+500 HP" },
                    new() { Name = "Amuleto do Dragão", Type = "Colar", Description = "Amuleto lendário que protege o portador contra golpes letais.", StatAfetada = "Defesa", StatBonus = "+30 Defesa" }
                };
                await context.Acessorios.AddRangeAsync(acessorios);
                await context.SaveChangesAsync();
            }
            // 4. POPULAR BUILDS DE EXEMPLO (se o fórum estiver vazio)
            if (!await context.Builds.AnyAsync())
            {
                var goku = await context.Personagens.FirstOrDefaultAsync(p => p.Name.Contains("Goku"));
                var ichigo = await context.Personagens.FirstOrDefaultAsync(p => p.Name.Contains("Ichigo"));
                var adminUser = await context.Utilizadores.FirstOrDefaultAsync(u => u.Id == "admin");
                var espada = await context.Armas.FirstOrDefaultAsync(a => a.Name.Contains("Zangetsu"));
                var luvas = await context.Armas.FirstOrDefaultAsync(a => a.Name.Contains("Luvas"));
                var mascara = await context.Acessorios.FirstOrDefaultAsync(a => a.Name.Contains("Máscara"));
                var brinco = await context.Acessorios.FirstOrDefaultAsync(a => a.Name.Contains("Potara"));
                if (goku != null && ichigo != null && adminUser != null)
                {
                    // Build 1: Goku Super Saiyajin
                    var buildGoku = new Build
                    {
                        Title = "Goku Super Saiyajin - Burst Físico",
                        Description = "<p>Esta build foca-se em <strong>destruição rápida</strong> com golpes corpo a corpo de alta intensidade e finalização com Kamehameha. Ideal para PvE agressivo!</p>",
                        Level = 85,
                        CharacterId = goku.Id,
                        UtilizadorID = adminUser.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-2),
                        UpdatedAt = DateTime.UtcNow.AddDays(-2),
                        Stats = new Estatisticas
                        {
                            Hp = 1200,
                            Strength = 95,
                            Defense = 80,
                            Magic = 70,
                            Endurance = 90,
                            Speed = 85
                        }
                    };
                    context.Builds.Add(buildGoku);
                    await context.SaveChangesAsync();
                    if (luvas != null)
                        context.BuildWeapons.Add(new BuildWeapon { BuildId = buildGoku.Id, WeaponId = luvas.Id, SlotPosition = 1 });
                    if (brinco != null)
                        context.BuildAccessories.Add(new BuildAccessory { BuildId = buildGoku.Id, AccessoryId = brinco.Id, SlotPosition = 1 });
                    // Build 2: Ichigo Bankai Crítico
                    var buildIchigo = new Build
                    {
                        Title = "Ichigo Hollow Bankai - Hiper Velocidade",
                        Description = "<p>Build desenhada para <em>velocidade extrema</em> e esquiva com cortes críticos de Zangetsu. Requer bom timing para não esgotar a estamina.</p>",
                        Level = 75,
                        CharacterId = ichigo.Id,
                        UtilizadorID = adminUser.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(-12),
                        UpdatedAt = DateTime.UtcNow.AddHours(-12),
                        Stats = new Estatisticas
                        {
                            Hp = 950,
                            Strength = 90,
                            Defense = 65,
                            Magic = 80,
                            Endurance = 70,
                            Speed = 100
                        }
                    };
                    context.Builds.Add(buildIchigo);
                    await context.SaveChangesAsync();
                    if (espada != null)
                        context.BuildWeapons.Add(new BuildWeapon { BuildId = buildIchigo.Id, WeaponId = espada.Id, SlotPosition = 1 });
                    if (mascara != null)
                        context.BuildAccessories.Add(new BuildAccessory { BuildId = buildIchigo.Id, AccessoryId = mascara.Id, SlotPosition = 1 });
                    // Adicionar um comentário de exemplo na Build do Goku
                    context.Comentarios.Add(new Comment
                    {
                        BuildId = buildGoku.Id,
                        UserId = adminUser.Id,
                        Content = "Grande build! Troquei um dos acessórios por defesa mágica e ajudou imenso contra bosses de feitiçaria.",
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    });
                    await context.SaveChangesAsync();
                }
            }
        }
    }

}
