﻿using BSFood.Apoio;
using BSFood.DataTransfer;
using BSFood.Models;

namespace BSFood.BusinessLogic.Interfaces
{
    public interface IClientes
    {
        Retorno RetornaCliente(int intCodigo, enNavegacao? enDirecao);

        Retorno RetornaListaCliente(string strCodigo, string strNome, string strTelefone, int intSkip, int intTake);

        Retorno SalvarCliente(tbCliente objCliente, int intFunCodigo);

        Retorno ExcluirCliente(int intCodigo);
    }
}
