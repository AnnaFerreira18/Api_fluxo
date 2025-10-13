using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityConfiguration
{
    public class AnotacaoMap : IEntityTypeConfiguration<Anotacao>
    {
        public void Configure(EntityTypeBuilder<Anotacao> builder)
        {
            builder.ToTable("Anotacao");

            builder.HasKey(a => a.IdAnotacao);

            builder.Property(a => a.Titulo)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(a => a.Conteudo)
                .IsRequired();

            builder.Property(a => a.Favorito)
                .IsRequired();

            builder.Property(a => a.Ordem)
                .IsRequired();

            builder.Property(a => a.Deletado)
                .IsRequired();

            builder.Property(a => a.DataDeletado)
                .IsRequired(false);

            builder.Property(u => u.DataCriacao)
                   .Metadata.SetDefaultValueSql("SYSDATETIME()");

            builder.Property(a => a.DataAtualizacao)
                .IsRequired();
        }
    }
}
