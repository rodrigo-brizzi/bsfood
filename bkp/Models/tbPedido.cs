﻿using System;
using System.Collections.Generic;

namespace BSFoodFramework.Models
{
    public class tbPedido
    {
        public int ped_codigo { get; set; }
        public DateTime? ped_data { get; set; }
        public DateTime? ped_dataEntrega { get; set; }
        public string ped_telefone { get; set; }
        public string ped_nomeCliente { get; set; }
        public string ped_logradouro { get; set; }
        public string ped_numero { get; set; }
        public string ped_bairro { get; set; }
        public string ped_cep { get; set; }
        public string ped_complemento { get; set; }
        public int ped_ordem { get; set; }
        public string ped_observacao { get; set; }
        public DateTime? ped_tempoEntrega { get; set; }
        public string ped_origem { get; set; }
        public string ped_status { get; set; }
        public int ped_quantidadePessoas { get; set; }
        public int ped_numeroMesa { get; set; }
        public decimal ped_valorSubTotal { get; set; }
        public decimal ped_valorTaxaEntrega { get; set; }
        public decimal ped_valorDespesa { get; set; }
        public decimal ped_valorDesconto { get; set; }
        public decimal ped_valorRecebido { get; set; }
        public decimal ped_valorTroco { get; set; }
        public decimal ped_valorTotal { get; set; }
        public string ped_motivoCancelamento { get; set; }
        public string ped_cobranca { get; set; }

        public int cli_codigo { get; set; }
        public virtual tbCliente tbCliente { get; set; }

        public int fun_codigo { get; set; }
        public virtual tbFuncionario tbFuncionario { get; set; }

        public int? fun_funcionarioEntregador { get; set; }
        public virtual tbFuncionario tbFuncionarioEntregador { get; set; }

        public int? cid_codigo { get; set; }
        public virtual tbCidade tbCidade { get; set; }

        public int? est_codigo { get; set; }
        public virtual tbEstado tbEstado { get; set; }

        public int? bai_codigo { get; set; }
        public virtual tbBairro tbBairro { get; set; }

        public int cai_codigo { get; set; }
        public virtual tbCaixa tbCaixa { get; set; }

        public int? fpg_codigo { get; set; }
        public virtual tbFormaPagamento tbFormaPagamento { get; set; }

        public virtual ICollection<tbMesa> tbMesa { get; set; }
        public virtual ICollection<tbPedidoProduto> tbPedidoProduto { get; set; }
        public virtual ICollection<tbVenda> tbVenda { get; set; }
    }
}
