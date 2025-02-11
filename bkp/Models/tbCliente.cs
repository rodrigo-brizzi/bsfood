﻿using System;
using System.Collections.Generic;

namespace BSFoodFramework.Models
{
    public class tbCliente
    {
        public int cli_codigo { get; set; }
        public string cli_nome { get; set; }
        public string cli_nomeFantasia { get; set; }
        public string cli_tipo { get; set; }
        public string cli_cpfCnpj { get; set; }
        public string cli_rgIe { get; set; }
        public string cli_sexo { get; set; }
        public string cli_email { get; set; }
        public string cli_observacao { get; set; }
        public int cli_fechamento { get; set; }
        public int cli_vencimento { get; set; }
        public decimal cli_limiteCredito { get; set; }
        public DateTime? cli_dataNascimento { get; set; }

        public int cgr_codigo { get; set; }
        public virtual tbClienteGrupo tbClienteGrupo { get; set; }

        public virtual ICollection<tbClienteEndereco> tbClienteEndereco { get; set; }
        public virtual ICollection<tbClienteTelefone> tbClienteTelefone { get; set; }
        public virtual ICollection<tbPedido> tbPedido { get; set; }
        public virtual ICollection<tbVenda> tbVenda { get; set; }
        public virtual ICollection<tbConfiguracao> tbConfiguracao { get; set; }
    }
}
