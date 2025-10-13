using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using Infrastructure.Context; // Certifique-se de que este namespace está correto

namespace Infrastructure.Factories
{
    // Esta classe ensina as ferramentas de linha de comando a criar o DbContext em tempo de design.
    public class FluxoDeNotasDbContextFactory : IDesignTimeDbContextFactory<FluxoDeNotasDbContext>
    {
        public FluxoDeNotasDbContext CreateDbContext(string[] args)
        {
            // Este bloco de código encontra e lê o seu ficheiro appsettings.json do projeto Api.
            // Ele assume que a pasta da API se chama 'Api'. Se tiver outro nome, ajuste aqui.
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Api"))
                .AddJsonFile("appsettings.json")
                .Build();

            // Cria um construtor de opções para o DbContext.
            var builder = new DbContextOptionsBuilder<FluxoDeNotasDbContext>();

            // Obtém a connection string do ficheiro de configuração.
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Configura o DbContext para usar o SQL Server com a connection string encontrada.
            builder.UseSqlServer(connectionString);

            // Retorna uma nova instância do seu DbContext com as opções configuradas.
            return new FluxoDeNotasDbContext(builder.Options);
        }
    }
}