﻿using BSFood.Apoio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BSFood.DataTransfer
{
    public class ConfigLocal
    {
        public bool blnSqlServer { get; set; }

        public bool blnSqlCompact { get; set; }

        public string strEnderecoBanco { get; set; }

        public string strNomeBanco { get; set; }

        public string strTerminal { get; set; }

        [XmlIgnore()]
        public enTipoBanco TipoBanco
        {
            get
            {
                if (blnSqlServer)
                    return enTipoBanco.SqlServer;
                else
                    return enTipoBanco.SqlCompact;
            }
        }
    }
}
