// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using Hummingbird.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AXA.WebEngine.ReportViewer
{
    /// <summary>
    /// Interaction logic for WindowShowImage.xaml
    /// </summary>
    public partial class WindowShowImage : BasicWindow
    {
        public WindowShowImage()
        {
            InitializeComponent();
        }

        private void image_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var bitmap = App.ConvertToBitmap(image, 96d);
            App.SaveImageToClipboard(bitmap);
            ShowToastNotification("The screenshot has been saved to clipboard", NotificationLevel.Information, null, 2);
        }

    }
}
