// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using Hummingbird.UI;
using System.Windows.Input;

namespace AxaFrance.WebEngine.ReportViewer
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
