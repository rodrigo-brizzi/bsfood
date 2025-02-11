﻿using System.Windows.Data;
using System.Windows.Media;
using BSFoodFramework.Apoio;

namespace BSFood.Apoio.Converters
{
    public class StatusCaixaToColorConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is string))
                return new SolidColorBrush(Colors.Black);

            var dValue = value.ToString();
            if (dValue == enStatusCaixa.Aberto.ToString())
                return new SolidColorBrush(Colors.Blue);
            else if (dValue == enStatusCaixa.Fechado.ToString())
                return new SolidColorBrush(Colors.Red);
            else
                return new SolidColorBrush(Colors.Blue);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
