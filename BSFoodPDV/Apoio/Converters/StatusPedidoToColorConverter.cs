﻿using System.Windows.Data;
using System.Windows.Media;
using BSFoodFramework.Apoio;

namespace BSFoodPDV.Apoio.Converters
{
    public class StatusPedidoToColorConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is string))
                return new SolidColorBrush(Colors.Black);

            var dValue = value.ToString();
            if (dValue == "Produção" || dValue == enStatusPedido.P.ToString())
                return new SolidColorBrush(Colors.Red);
            else if (dValue == "Entrega" || dValue == enStatusPedido.E.ToString())
                return new SolidColorBrush(Colors.Blue);
            else if (dValue == "Finalizado" || dValue == enStatusPedido.F.ToString())
                return new SolidColorBrush(Colors.Green);
            else if (dValue == "Excluído" || dValue == enStatusPedido.X.ToString())
                return new SolidColorBrush(Colors.Silver);
            else
                return new SolidColorBrush(Colors.Red);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
