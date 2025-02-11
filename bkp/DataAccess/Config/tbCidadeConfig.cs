﻿using BSFoodFramework.Models;
using System.Data.Entity.ModelConfiguration;

namespace BSFoodFramework.DataAccess.Config
{
    public class tbCidadeConfig : EntityTypeConfiguration<tbCidade>
    {
        public tbCidadeConfig()
        {
            this.ToTable("tbCidade");
            this.HasKey(e => e.cid_codigo);

            this.Property(e => e.cid_codigo).HasColumnName("cid_codigo");
            this.Property(e => e.cid_ibge).HasColumnName("cid_ibge").HasMaxLength(7);
            this.Property(e => e.cid_nome).HasColumnName("cid_nome").HasMaxLength(60);
            this.HasRequired(e => e.tbEstado).WithMany(e => e.tbCidade).HasForeignKey(e => e.est_codigo);
            this.Property(e => e.est_codigo).HasColumnName("est_codigo");
        }
    }
}
