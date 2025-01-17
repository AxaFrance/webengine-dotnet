// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace AxaFrance.WebEngine.ReportViewer
{
    public class ResultToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var Result = value.ToString();
            if (string.IsNullOrEmpty(Result))
            {
                return new BitmapImage(new Uri("pack://application:,,,/ReportViewer;component/Icons/icon_Ignored.png", UriKind.RelativeOrAbsolute));
            }
            else
            {
                return new BitmapImage(new Uri("pack://application:,,,/ReportViewer;component/Icons/icon_" + Result + ".png", UriKind.RelativeOrAbsolute));
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
