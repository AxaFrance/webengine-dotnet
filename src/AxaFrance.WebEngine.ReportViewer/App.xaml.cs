// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;


namespace AxaFrance.WebEngine.ReportViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static string LogFile { get; set; }
        public static string Filter { get; set; }
        public static bool FilterFailed { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.StartupUri = new Uri("MainWindow.xaml", UriKind.RelativeOrAbsolute);
            if (e.Args.Length > 0)
            {
                LogFile = e.Args[0];
            }
            LoadSyntaxConfig();
        }

        private static void LoadSyntaxConfig()
        {
            using (var streamJsDark = Assembly.GetExecutingAssembly().GetManifestResourceStream($"AxaFrance.WebEngine.ReportViewer.XmlDark.xshd"))
            {
                using (XmlReader readerJsDark = XmlReader.Create(streamJsDark))
                {
                    var definitionJsDark = HighlightingLoader.Load(readerJsDark, HighlightingManager.Instance);
                    HighlightingManager.Instance.RegisterHighlighting("XML_DARK", new[] { "XML_DARK" }, definitionJsDark);
                    readerJsDark.Close();
                }
            }
        }


        internal static RenderTargetBitmap ConvertToBitmap(UIElement uiElement, double resolution)
        {
            var scale = resolution / 96d;

            uiElement.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            var sz = uiElement.DesiredSize;
            var rect = new Rect(sz);
            uiElement.Arrange(rect);

            var bmp = new RenderTargetBitmap((int)(scale * (rect.Width)), (int)(scale * (rect.Height)), scale * 96, scale * 96, PixelFormats.Default);
            bmp.Render(uiElement);

            return bmp;
        }

        internal static void SaveImageToClipboard(RenderTargetBitmap bitmap)
        {
            Clipboard.SetImage(bitmap);
        }
    }
}
