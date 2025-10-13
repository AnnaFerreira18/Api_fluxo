using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfiguration
{
    public class ProjetoMap : IEntityTypeConfiguration<Projeto>
    {
        public void Configure(EntityTypeBuilder<Projeto> builder)
        {
            builder.ToTable("Projeto");

            builder.HasKey(p => p.IdProjeto);

            builder.Property(p => p.Nome)
                .IsRequired() 
                .HasMaxLength(255);

            builder.Property(p => p.Descricao)
                .IsRequired(false); 

            builder.Property(p => p.Deletado)
                .IsRequired();

            builder.Property(p => p.DataDeletado)
                .IsRequired(false);

            builder.Property(u => u.DataCriacao)
                   .Metadata.SetDefaultValueSql("SYSDATETIME()");

            builder.Property(p => p.DataAtualizacao)
                .IsRequired();

            builder.HasMany(p => p.Anotacoes)
                   .WithOne(a => a.Projeto) 
                   .HasForeignKey(a => a.IdProjeto);
        }
    }
}
