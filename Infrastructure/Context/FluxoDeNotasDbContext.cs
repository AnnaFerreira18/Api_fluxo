using Domain.Entities; 
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace Infrastructure.Context
{
    public class FluxoDeNotasDbContext : DbContext
    {
        // O construtor recebe as opções de configuração (como a connection string)
        // que são passadas a partir do projeto da API.
        public FluxoDeNotasDbContext(DbContextOptions<FluxoDeNotasDbContext> options) : base(options){}

        // Cada propriedade DbSet<T> representa uma tabela que será criada
        // no banco de dados, baseada na entidade T.
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Projeto> Projetos { get; set; }
        public DbSet<Anotacao> Anotacoes { get; set; }
        public DbSet<Tarefa> Tarefas { get; set; }
        public DbSet<CodigoTemporario> CodigosTemporarios { get; set; }

        // Este método é chamado pelo Entity Framework Core quando o modelo
        // para o contexto está a ser criado pela primeira vez.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Esta linha é muito poderosa. Ela varre todo o projeto (assembly)
            // da Infrastructure à procura de classes que implementam IEntityTypeConfiguration
            // (como a UsuarioConfiguration que planeámos) e aplica todas as suas
            // configurações de uma só vez. Isto mantém este ficheiro limpo.

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //    modelBuilder
        //        .HasDbFunction(typeof(AppDbContext).GetMethod(nameof(FormatarData), new[] { typeof(DateTime), typeof(int) }))
        //        .HasTranslation(args => new SqlFunctionExpression(
        //            "convert",
        //            args.Prepend(new SqlFragmentExpression("varchar(10)")),
        //            true,
        //            new[] { false, true, false },
        //            typeof(string),
        //            null));

        //    modelBuilder.ApplyConfiguration(new ColaboradorMap());
        //    modelBuilder.ApplyConfiguration(new AulaMap());
        //    modelBuilder.ApplyConfiguration(new InscricaoMap());
        //    modelBuilder.ApplyConfiguration(new HorarioMap());


        //}
        //public string FormatarData(DateTime data, int style)
        //  => throw new NotSupportedException();
    }
}
