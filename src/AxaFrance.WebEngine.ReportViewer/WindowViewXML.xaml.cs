// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using Hummingbird.UI;
using ICSharpCode.AvalonEdit.Folding;

namespace AxaFrance.WebEngine.ReportViewer
{
    /// <summary>
    /// Logique d'interaction pour WindowViewXML.xaml
    /// </summary>
    public partial class WindowViewXML : BasicWindow
    {
        public WindowViewXML(string text)
        {
            InitializeComponent();
            txtXmlReport.Text = text;
            var foldingManager = FoldingManager.Install(txtXmlReport.TextArea);
            var foldingStrategy = new XmlFoldingStrategy();
            foldingStrategy.UpdateFoldings(foldingManager, txtXmlReport.Document);
        }
    }
}
