﻿using System.Windows;
using System.Windows.Controls;

namespace BSFood.Apoio.Behavior
{
    public static class BrowserBehavior
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
                "Html",
                typeof(string),
                typeof(BrowserBehavior),
                new FrameworkPropertyMetadata(OnHtmlChanged));

        [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
        public static string GetHtml(WebBrowser d)
        {
            return (string)d.GetValue(HtmlProperty);
        }

        public static void SetHtml(WebBrowser d, string value)
        {
            d.SetValue(HtmlProperty, value);
        }

        static void OnHtmlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var webBrowser = dependencyObject as WebBrowser;
            //webBrowser.NavigateToString(e.NewValue as string ?? "&nbsp;");
            if (webBrowser == null) return;
            var strUrl = e.NewValue as string;
            if (!string.IsNullOrWhiteSpace(strUrl))
                webBrowser.NavigateToString(strUrl);
        }
    }
}
