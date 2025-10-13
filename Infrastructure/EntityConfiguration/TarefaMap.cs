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
    public class TarefaMap : IEntityTypeConfiguration<Tarefa>
    {
        public void Configure(EntityTypeBuilder<Tarefa> builder)
        {
            builder.ToTable("Tarefa");
            builder.HasKey(t => t.IdTarefa);

            builder.Property(t => t.Texto)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(t => t.Concluido)
                .IsRequired();

            builder.Property(t => t.Ordem)
                .IsRequired();

            builder.Property(t => t.Deletado)
                .IsRequired();

            builder.Property(t => t.DataDeletado)
                .IsRequired(false);

            builder.Property(u => u.DataCriacao)
                .Metadata.SetDefaultValueSql("SYSDATETIME()");

            builder.Property(t => t.DataAtualizacao)
                .IsRequired();
        }
    }
}
