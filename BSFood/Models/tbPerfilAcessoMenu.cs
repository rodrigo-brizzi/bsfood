﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BSFood.Models
{
    public class tbPerfilAcessoMenu
    {
        public bool pam_permiteAlteracao { get; set; }
        public bool pam_permiteExclusao { get; set; }
        public bool pam_permiteInclusao { get; set; }
        public bool pam_permiteVisualizacao { get; set; }
        public bool pam_toolBar { get; set; }

        public int men_codigo { get; set; }
        public virtual tbMenu tbMenu { get; set; }

        public int pac_codigo { get; set; }
        public virtual tbPerfilAcesso tbPerfilAcesso { get; set; }
    }
}
