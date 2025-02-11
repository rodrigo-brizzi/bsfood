﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFood.Models
{
    public class tbEstado
    {
        public int est_codigo { get; set; }
        public string est_sigla { get; set; }
        public string est_nome { get; set; }

        public virtual ICollection<tbCidade> tbCidade { get; set; }
        public virtual ICollection<tbClienteEndereco> tbClienteEndereco { get; set; }
        public virtual ICollection<tbEmpresa> tbEmpresa { get; set; }
        public virtual ICollection<tbFuncionario> tbFuncionario { get; set; }
        public virtual ICollection<tbFornecedor> tbFornecedor { get; set; }
        public virtual ICollection<tbPedido> tbPedido { get; set; }
    }
}
