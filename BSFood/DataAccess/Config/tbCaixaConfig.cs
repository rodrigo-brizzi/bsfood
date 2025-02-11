﻿using BSFood.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFood.DataAccess.Config
{
    public class tbCaixaConfig : EntityTypeConfiguration<tbCaixa>
    {
        public tbCaixaConfig()
        {
            this.ToTable("tbCaixa");
            this.HasKey(e => e.cai_codigo);

            this.Property(e => e.cai_codigo).HasColumnName("cai_codigo");
            this.Property(e => e.cai_dataAbertura).HasColumnName("cai_dataAbertura");
            this.Property(e => e.cai_dataFechamento).HasColumnName("cai_dataFechamento");
            this.Property(e => e.cai_observacao).HasColumnName("cai_observacao").HasMaxLength(250);
            this.Property(e => e.cai_ordemPedido).HasColumnName("cai_ordemPedido");
            this.HasRequired(e => e.tbFuncionario).WithMany(e => e.tbCaixa).HasForeignKey(e => e.fun_codigo);
            this.Property(e => e.fun_codigo).HasColumnName("fun_codigo");
        }
    }
}
