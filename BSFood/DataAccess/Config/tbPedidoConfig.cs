﻿using BSFood.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFood.DataAccess.Config
{
    public class tbPedidoConfig : EntityTypeConfiguration<tbPedido>
    {
        public tbPedidoConfig()
        {
            this.ToTable("tbPedido");
            this.HasKey(e => e.ped_codigo);

            this.Property(e => e.ped_codigo).HasColumnName("ped_codigo");
            this.Property(e => e.ped_data).HasColumnName("ped_data");
            this.Property(e => e.ped_dataEntrega).HasColumnName("ped_dataEntrega");
            this.Property(e => e.ped_telefone).HasColumnName("ped_telefone").HasMaxLength(20);
            this.Property(e => e.ped_nomeCliente).HasColumnName("ped_nomeCliente").HasMaxLength(100);
            this.Property(e => e.ped_logradouro).HasColumnName("ped_logradouro").HasMaxLength(200);
            this.Property(e => e.ped_numero).HasColumnName("ped_numero").HasMaxLength(10);
            this.Property(e => e.ped_bairro).HasColumnName("ped_bairro").HasMaxLength(100);
            this.Property(e => e.ped_cep).HasColumnName("ped_cep").HasMaxLength(9);
            this.Property(e => e.ped_complemento).HasColumnName("ped_complemento").HasMaxLength(150);
            this.Property(e => e.ped_ordem).HasColumnName("ped_ordem");
            this.Property(e => e.ped_observacao).HasColumnName("ped_observacao").HasMaxLength(250);
            this.Property(e => e.ped_tempoEntrega).HasColumnName("ped_tempoEntrega");
            this.Property(e => e.ped_origem).HasColumnName("ped_origem").HasMaxLength(1);
            this.Property(e => e.ped_status).HasColumnName("ped_status").HasMaxLength(1);
            this.Property(e => e.ped_quantidadePessoas).HasColumnName("ped_quantidadePessoas");
            this.Property(e => e.ped_numeroMesa).HasColumnName("ped_numeroMesa");
            this.Property(e => e.ped_valorSubTotal).HasColumnName("ped_valorSubTotal");
            this.Property(e => e.ped_valorTaxaEntrega).HasColumnName("ped_valorTaxaEntrega");
            this.Property(e => e.ped_valorDespesa).HasColumnName("ped_valorDespesa");
            this.Property(e => e.ped_valorDesconto).HasColumnName("ped_valorDesconto");
            this.Property(e => e.ped_valorRecebido).HasColumnName("ped_valorRecebido");
            this.Property(e => e.ped_valorTroco).HasColumnName("ped_valorTroco");
            this.Property(e => e.ped_valorTotal).HasColumnName("ped_valorTotal");
            this.Property(e => e.ped_motivoCancelamento).HasColumnName("ped_motivoCancelamento").HasMaxLength(250);
            this.Property(e => e.ped_cobranca).HasColumnName("ped_cobranca").HasMaxLength(1);
            this.HasRequired(e => e.tbCliente).WithMany(e => e.tbPedido).HasForeignKey(e => e.cli_codigo);
            this.Property(e => e.cli_codigo).HasColumnName("cli_codigo");
            this.HasRequired(e => e.tbFuncionario).WithMany(e => e.tbPedido).HasForeignKey(e => e.fun_codigo);
            this.Property(e => e.fun_codigo).HasColumnName("fun_codigo");
            this.HasOptional(e => e.tbFuncionarioEntregador).WithMany(e => e.tbPedidoEntregador).HasForeignKey(e => e.fun_funcionarioEntregador);
            this.Property(e => e.fun_funcionarioEntregador).HasColumnName("fun_funcionarioEntregador");
            this.HasOptional(e => e.tbCidade).WithMany(e => e.tbPedido).HasForeignKey(e => e.cid_codigo);
            this.Property(e => e.cid_codigo).HasColumnName("cid_codigo");
            this.HasOptional(e => e.tbEstado).WithMany(e => e.tbPedido).HasForeignKey(e => e.est_codigo);
            this.Property(e => e.est_codigo).HasColumnName("est_codigo");
            this.HasOptional(e => e.tbBairro).WithMany(e => e.tbPedido).HasForeignKey(e => e.bai_codigo);
            this.Property(e => e.bai_codigo).HasColumnName("bai_codigo");
            this.HasRequired(e => e.tbCaixa).WithMany(e => e.tbPedido).HasForeignKey(e => e.cai_codigo);
            this.Property(e => e.cai_codigo).HasColumnName("cai_codigo");
            this.HasOptional(e => e.tbFormaPagamento).WithMany(e => e.tbPedido).HasForeignKey(e => e.fpg_codigo);
            this.Property(e => e.fpg_codigo).HasColumnName("fpg_codigo");
        }
    }
}
