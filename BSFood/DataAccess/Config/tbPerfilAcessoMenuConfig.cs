﻿using BSFood.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFood.DataAccess.Config
{
    public class tbPerfilAcessoMenuConfig : EntityTypeConfiguration<tbPerfilAcessoMenu>
    {
        public tbPerfilAcessoMenuConfig()
        {
            this.ToTable("tbPerfilAcessoMenu");
            this.HasKey(e => new { e.men_codigo, e.pac_codigo });

            this.Property(e => e.pam_permiteAlteracao).HasColumnName("pam_permiteAlteracao");
            this.Property(e => e.pam_permiteExclusao).HasColumnName("pam_permiteExclusao");
            this.Property(e => e.pam_permiteInclusao).HasColumnName("pam_permiteInclusao");
            this.Property(e => e.pam_permiteVisualizacao).HasColumnName("pam_permiteVisualizacao");
            this.Property(e => e.pam_toolBar).HasColumnName("pam_toolBar");
            this.HasRequired(e => e.tbMenu).WithMany(e => e.tbPerfilAcessoMenu).HasForeignKey(e => e.men_codigo);
            this.Property(e => e.men_codigo).HasColumnName("men_codigo");
            this.HasRequired(e => e.tbPerfilAcesso).WithMany(e => e.tbPerfilAcessoMenu).HasForeignKey(e => e.pac_codigo);
            this.Property(e => e.pac_codigo).HasColumnName("pac_codigo");
        }
    }
}
