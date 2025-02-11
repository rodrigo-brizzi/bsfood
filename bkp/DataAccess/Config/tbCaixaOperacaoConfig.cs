﻿using BSFoodFramework.Models;
using System.Data.Entity.ModelConfiguration;

namespace BSFoodFramework.DataAccess.Config
{
    public class tbCaixaOperacaoConfig : EntityTypeConfiguration<tbCaixaOperacao>
    {
        public tbCaixaOperacaoConfig()
        {
            this.ToTable("tbCaixaOperacao");
            this.HasKey(e => e.caio_codigo);

            this.Property(e => e.caio_codigo).HasColumnName("caio_codigo");
            this.Property(e => e.caio_descricao).HasColumnName("caio_descricao").HasMaxLength(100);
            this.Property(e => e.caio_tipoOperacao).HasColumnName("caio_tipoOperacao").HasMaxLength(1);
        }
    }
}
