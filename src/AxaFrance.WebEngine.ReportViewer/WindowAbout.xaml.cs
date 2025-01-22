// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using Hummingbird.UI;
using System.Windows;

namespace AxaFrance.WebEngine.ReportViewer
{
    /// <summary>
    /// Interaction logic for WindowAbout.xaml
    /// </summary>
    public partial class WindowAbout : BasicWindow
    {
        public WindowAbout()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
