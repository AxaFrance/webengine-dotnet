// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System.Windows;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;

namespace AxaFrance.WebEngine.ReportViewer
{
    /// <summary>
    /// Logique d'interaction pour WindowViewXML.xaml
    /// </summary>
    public partial class WindowViewXML : Window
    {
        public WindowViewXML(string text)
        {
            InitializeComponent();
            txtXmlReport.Text = text;
            var foldingManager = FoldingManager.Install(txtXmlReport.TextArea);
            var foldingStrategy = new XmlFoldingStrategy();
            foldingStrategy.UpdateFoldings(foldingManager, txtXmlReport.Document);
            ApplySyntaxTheme();
            App.ThemeChanged += OnThemeChanged;
            Closed += (_, _) => App.ThemeChanged -= OnThemeChanged;
        }

        private void OnThemeChanged(bool lightMode) => ApplySyntaxTheme();

        private void ApplySyntaxTheme()
        {
            var name = App.IsWindowsLightMode() ? "XML_LIGHT" : "XML_DARK";
            txtXmlReport.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition(name);
        }
    }
}
