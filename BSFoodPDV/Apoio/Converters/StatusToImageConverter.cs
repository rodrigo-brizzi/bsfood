﻿using System.Windows.Data;

namespace BSFoodPDV.Apoio.Converters
{
    public class StatusToImageConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is string))
                return "/BSFood;component/Imagens/mesalivre450.png";

            var dValue = value.ToString();
            if (dValue == "Produção" || dValue == "P" || dValue == "Aberto")
                return "/BSFood;component/Imagens/mesalivre450.png";
            else if (dValue == "Entrega" || dValue == "E")
                return "/BSFood;component/Imagens/mesalivre450.png";
            else if (dValue == "Finalizado" || dValue == "F" || dValue == "Livre" || dValue == "L")
                return "/BSFood;component/Imagens/mesalivre450.png";
            else if (dValue == "Excluído" || dValue == "X" || dValue == "Fechado" || dValue == "Ocupado" || dValue == "O")
                return "/BSFood;component/Imagens/mesaocupada450.png";
            else
                return "/BSFood;component/Imagens/mesalivre450.png";
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
