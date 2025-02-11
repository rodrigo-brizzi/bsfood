﻿using BSFood.Apoio;
using BSFood.DataTransfer;
using BSFood.Models;

namespace BSFood.BusinessLogic.Interfaces
{
    public interface IBairros
    {
        Retorno RetornaBairro(int intCodigo, enNavegacao? enDirecao);

        Retorno RetornaListaBairro(string strCodigo, string strNome, int intSkip, int intTake);

        Retorno RetornaListaBairro();

        Retorno SalvarBairro(tbBairro objBairro, int intFunCodigo);

        Retorno ExcluirBairro(int intCodigo);
    }
}
