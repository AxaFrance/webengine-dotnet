// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Source - https://stackoverflow.com/a/79185915
// Posted by MHolzmayr
// License - CC BY-SA 4.0
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace AxaFrance.WebEngine.ReportViewer
{
    /// <summary>Helper converter for drawing connected lines between TreeView items.</summary>
    public class TreeViewLineConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TreeViewItem item = (TreeViewItem)value;
            ItemsControl ic = ItemsControl.ItemsControlFromItemContainer(item);
            return ic.ItemContainerGenerator.IndexFromContainer(item) == ic.Items.Count - 1;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
