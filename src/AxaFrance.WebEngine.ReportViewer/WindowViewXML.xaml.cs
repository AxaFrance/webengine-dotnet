// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using Hummingbird.UI;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
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
