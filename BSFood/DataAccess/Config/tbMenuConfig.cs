﻿using BSFood.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFood.DataAccess.Config
{
    public class tbMenuConfig : EntityTypeConfiguration<tbMenu>
    {
        public tbMenuConfig()
        {
            this.ToTable("tbMenu");
            this.HasKey(e => e.men_codigo);

            this.Property(e => e.men_codigo).HasColumnName("men_codigo");
            this.Property(e => e.men_nivel).HasColumnName("men_nivel");
            this.Property(e => e.men_nomeControle).HasColumnName("men_nomeControle").HasMaxLength(100);
            this.Property(e => e.men_cabecalho).HasColumnName("men_cabecalho").HasMaxLength(35);
            this.Property(e => e.men_ordem).HasColumnName("men_ordem").HasMaxLength(50);
            this.Property(e => e.men_imagem).HasColumnName("men_imagem").HasMaxLength(50);
            this.Property(e => e.men_status).HasColumnName("men_status");
            //this.HasRequired(e => e.MenuPai).WithMany(e => e.MenuPais).HasForeignKey(e => e.CodigoMenu);
            this.HasOptional(e => e.tbMenuPai).WithMany(e => e.tbMenuFilho).HasForeignKey(e => e.men_codigoPai);
            this.Property(e => e.men_codigoPai).HasColumnName("men_codigoPai");
        }
    }
}
