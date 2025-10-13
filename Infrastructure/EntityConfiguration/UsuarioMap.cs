using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfiguration
{
    public class UsuarioMap : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuario");

            builder.HasKey(u => u.IdUsuario);

            builder.Property(u => u.Nome)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.Telefone)
                .IsRequired(false)
                .HasMaxLength(20);

            builder.Property(u => u.Senha)
                .IsRequired();

            builder.Property(u => u.EmailVerificado)
                .IsRequired();

            builder.Property(u => u.UltimoLogin).IsRequired(false);

            builder.Property(u => u.DataCriacao)
                   .Metadata.SetDefaultValueSql("SYSDATETIME()");

            builder.Property(u => u.DataAtualizacao).IsRequired();
            // --- Relacionamentos ---
            builder.HasMany(u => u.Projetos)
                   .WithOne(p => p.Usuario)
                   .HasForeignKey(p => p.IdUsuario);

            builder.HasMany(u => u.Tarefas)
                   .WithOne(t => t.Usuario)
                   .HasForeignKey(t => t.IdUsuario);

            builder.HasMany(u => u.CodigosTemporarios)
                   .WithOne(c => c.Usuario)
                   .HasForeignKey(c => c.IdUsuario);
        }
    }
}