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
    public class CodigoVerificacaoMap : IEntityTypeConfiguration<CodigoTemporario>
    {
        public void Configure(EntityTypeBuilder<CodigoTemporario> builder)
        {
            builder.ToTable("CodigosTemporarios");
            builder.HasKey(c => c.IdCodigoTemporario);

            builder.Property(c => c.TipoDeCodigo)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.CodigoHash)
                .IsRequired(false); 

            builder.Property(c => c.DataExpiracao)
                .IsRequired();

            builder.Property(c => c.DataUtilizacao)
                .IsRequired(false); 
        }
    }
}
