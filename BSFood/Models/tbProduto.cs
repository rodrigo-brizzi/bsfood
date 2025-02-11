﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFood.Models
{
    public class tbProduto
    {
        public int pro_codigo { get; set; }
        public string pro_nome { get; set; }
        public bool pro_estoqueNegativo { get; set; }
        public decimal pro_precoEntrega { get; set; }
        public decimal pro_precoBalcao { get; set; }
        public string pro_observacao { get; set; }

        public int pgr_codigo { get; set; }
        public virtual tbProdutoGrupo tbProdutoGrupo { get; set; }

        public int psgr_codigo { get; set; }
        public virtual tbProdutoSubGrupo tbProdutoSubGrupo { get; set; }

        public virtual ICollection<tbPedidoProduto> tbPedidoProduto { get; set; }
        public virtual ICollection<tbVendaProduto> tbVendaProduto { get; set; }
    }
}
