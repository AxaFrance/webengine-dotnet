// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-8-1 10:22
using AxaFrance.WebEngine.Report;
using AxaFrance.WebEngine.Web;
using Hummingbird.UI;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace AxaFrance.WebEngine.ReportViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : BasicWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        bool hasLoaded = false;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!hasLoaded)
            {
                hasLoaded = true;
                Load();
            }
        }

        string xmlReport;
        private bool ShouldSkip(TestCaseReport tc)
        {
            if (!string.IsNullOrEmpty(App.Filter))
            {
                if (tc.TestName != null && tc.TestName.IndexOf(App.Filter, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            if (App.FailedTestOnly && tc.Result != Result.Failed && tc.Result != Result.CriticalError)
            {
                return true;
            }

            return false;
        }

        private void Load()
        {
            string filename = App.LogFile;
            if (string.IsNullOrEmpty(filename))
            {
                emptyPresenter.Visibility = Visibility.Visible;
                return;
            }
            try
            {

                XmlSerializer serializer = new XmlSerializer(typeof(AxaFrance.WebEngine.Report.TestSuiteReport));
                using (StreamReader sr = new StreamReader(filename))
                {
                    xmlReport = sr.ReadToEnd();
                }
                currentReport = (TestSuiteReport)serializer.Deserialize(new StreamReader(filename));
                txtSystemOut.Text = currentReport.SystemOut + "\n" + currentReport.SystemError;

                double totalSeconds = currentReport.Duration.TotalSeconds;
                ApplyFilter();
                pgPassrate.Value = currentReport.Failed;
                pgPassrate.Maximum = currentReport.Passed + currentReport.Failed;
                pgPassrate.ToolTip = $"{currentReport.Passed} passed, {currentReport.Failed} failed";
                IntializeErrorStatistic();
                emptyPresenter.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ShowMessageBox("Error", $"Unable to load the report file from {filename}: {ex.Message}", MessageBoxButton.OK, MessageBoxImage.Error);
                emptyPresenter.Visibility = Visibility.Visible;
            }
        }

        private void ApplyFilter()
        {
            lvTestcases.Items.Clear();
            foreach (var tc in currentReport.TestResult)
            {
                if (this.ShouldSkip(tc))
                {
                    continue;
                }
                lvTestcases.Items.Add(tc);
            }
            if (lvTestcases.Items.Count > 0)
            {
                lvTestcases.SelectedIndex = 0;
            }
            txtTitle.Text = $"Test cases ({lvTestcases.Items.Count}/{currentReport.TestResult.Count})";
        }

        private TestSuiteReport currentReport;

        private void IntializeErrorStatistic()
        {
            //TODO: This is a placeholder for the error statistic 
            int passed = 0, ignored = 0, failed = 0, none = 0;
            passed = currentReport.TestResult.Count(x => x.Result == Result.Passed);
            ignored = currentReport.TestResult.Count(x => x.Result == Result.Ignored);
            failed = currentReport.TestResult.Count(x => x.Result == Result.Failed || x.Result == Result.CriticalError);
            none = currentReport.TestResult.Count(x => x.Result == Result.None);

        }

        private void lvTestcases_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;

            lvTeststeps.Items.Clear();
            TestCaseReport tc = e.AddedItems[0] as TestCaseReport;
            spScreenShots.Children.Clear();
            lbTestData.DataContext = tc.TestData;

            lbTestData.Items.Clear();
            if (tc.TestData != null)
            {
                foreach (var c in tc.TestData)
                {
                    lbTestData.Items.Add(c);
                }
            }
            lbTestData.Visibility = lbTestData.Items.Count == 0 ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;

            if (tc.ActionReports != null)
            {
                FillActionReports(tc);
            }

            if (tc.AttachedData.FirstOrDefault(x => x.Name == "AccessibilityReport") != null)
            {
                var data = tc.AttachedData.FirstOrDefault(x => x.Name == "AccessibilityReport")?.Value;
                OpenAccessibilityReport(data);
                tabAccessibility.Visibility = Visibility.Visible;
            }
            else
            {
                tabAccessibility.Visibility = Visibility.Collapsed;
                tabControl.SelectedIndex = 0;
            }

            if (tc.AttachedData.FirstOrDefault(x => x.Name == "ResourceUsage") != null)
            {
                tabResourceUsages.Visibility = Visibility.Visible;
                var data = tc.AttachedData.FirstOrDefault(x => x.Name == "ResourceUsage")?.Value;
                string json = System.Text.Encoding.UTF8.GetString(data);
                var resourceUsage = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, NetworkRequest>>(json);
                dgImpacts.ItemsSource = resourceUsage.Values.ToArray();
                pcUsage.Series = GetResourcesSeries(resourceUsage);
                pcHttpCode.Series = GetHttpCodeSeries(resourceUsage);
                lblDownloadSize.Text = GetSize(resourceUsage.Values.Where(x=>x.IsCached == false).Sum(x => x.Reponse));
                lblTotalResources.Text = resourceUsage.Count.ToString();
            }
            else
            {
                tabResourceUsages.Visibility = Visibility.Collapsed;
            }


        }

        private string GetSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private IEnumerable<ISeries> GetHttpCodeSeries(Dictionary<string, NetworkRequest> resourceUsage)
        {
            Dictionary<long, long> response = new Dictionary<long, long>();
            foreach (var kvp in resourceUsage)
            {
                var code = kvp.Value.StatusCode;
                if (response.ContainsKey(code))
                {
                    response[code]++;
                }
                else
                {
                    response[code] = 1;
                }
            }

            List<ISeries> Series = new List<ISeries>();
            foreach (var item in response)
            {
                Series.Add(new PieSeries<long>
                {
                    Values = new long[] { item.Value },
                    Name = item.Key.ToString(),
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer,
                    DataLabelsSize = 15,
                    DataLabelsFormatter = value => $"{value.Label} : {value.Coordinate.PrimaryValue}",
                    ToolTipLabelFormatter = value => $"{value.Label} : {value.Coordinate.PrimaryValue}",
                });
            }
            return Series;
        }

        private IEnumerable<ISeries> GetResourcesSeries(Dictionary<string, NetworkRequest> resourceUsage)
        {
            Dictionary<string, long> data = new Dictionary<string, long>();
            foreach (var item in resourceUsage)
            {
                if (item.Value.IsCached) continue;
                if (data.ContainsKey(item.Value.ResourceType))
                {
                    data[item.Value.ResourceType] += item.Value.Reponse;
                }
                else
                {
                    data.Add(item.Value.ResourceType, item.Value.Reponse);
                }
            }

            List<ISeries> Series = new List<ISeries>();
            foreach (var item in data)
            {
                Series.Add(new PieSeries<long>
                {
                    Values = new long[] { item.Value },
                    Name = item.Key,
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer,
                    DataLabelsSize = 15,
                    DataLabelsFormatter = value => $"{value.Label} : {value.Coordinate.PrimaryValue} Bytes",
                    ToolTipLabelFormatter = value => $"{value.Label} : {value.Coordinate.PrimaryValue} Bytes",
                });
            }
            return Series;
        }

        private void FillActionReports(TestCaseReport tc)
        {
            foreach (var step in tc.ActionReports)
            {
                StackPanel sp = new StackPanel()
                {
                    Orientation = System.Windows.Controls.Orientation.Horizontal,
                    Margin = new Thickness(4),
                };
                ImageSource source;
                if (step.Result != Result.None)
                {
                    source = new BitmapImage(new Uri("/Icons/icon_" + step.Result + ".png", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    source = new BitmapImage(new Uri("/Icons/icon_Ignored.png", UriKind.RelativeOrAbsolute));
                }
                sp.Children.Add(new Image() { Source = source, Margin = new Thickness(4, 0, 4, 0) });

                sp.Children.Add(new TextBlock()
                {
                    Text = step.Name ?? "(Unnamed step)"
                });

                //checks if the step has screenshots. If so, add a camera icon
                if (step.Screenshots?.Count > 0)
                {
                    sp.Children.Add(new Image()
                    {
                        Source = new BitmapImage(new Uri("/Icons/icon_Camera.png", UriKind.RelativeOrAbsolute)),
                        Margin = new Thickness(4, 0, 4, 0),
                        Width = 16,
                        Height = 16,
                    });
                }

                sp.Children.Add(new TextBlock()
                {
                    Text = step.DurationText,
                    Margin = new Thickness(8, 0, 4, 0),
                    Foreground = new SolidColorBrush(Colors.Gray),
                });

                TreeViewItem tvi = new TreeViewItem()
                {
                    Header = sp
                };
                tvi.Tag = step;
                lvTeststeps.Items.Add(tvi);
                if (step.SubActionReports != null && step.SubActionReports.Count > 0)
                {
                    FillTreeViewItem(tvi, step);
                }
            }
        }

        private void FillTreeViewItem(TreeViewItem parent, ActionReport action)
        {
            foreach (var a in action.SubActionReports)
            {
                StackPanel sp = new StackPanel()
                {
                    Orientation = System.Windows.Controls.Orientation.Horizontal,
                    Margin = new Thickness(4),
                };

                ImageSource source;

                switch (a.Result)
                {
                    case Result.Passed:
                        source = new BitmapImage(new Uri("/Icons/icon_Passed.png", UriKind.RelativeOrAbsolute));
                        break;
                    case Result.Failed:
                        source = new BitmapImage(new Uri("/Icons/icon_Failed.png", UriKind.RelativeOrAbsolute));
                        break;
                    default:
                        source = new BitmapImage(new Uri("/Icons/icon_" + a.Result + ".png", UriKind.RelativeOrAbsolute));
                        break;
                }


                sp.Children.Add(new Image() { Source = source, Margin = new Thickness(4, 0, 4, 0) });

                sp.Children.Add(new TextBlock()
                {
                    Text = a.Name ?? "(Unnamed action)",
                });

                sp.Children.Add(new TextBlock()
                {
                    Text = a.DurationText,
                    Margin = new Thickness(8, 0, 4, 0),
                    Foreground = new SolidColorBrush(Colors.Gray),
                });

                TreeViewItem currentLevel = new TreeViewItem()
                {
                    Header = sp
                };
                currentLevel.Tag = a;
                parent.Items.Add(currentLevel);

                if (a.SubActionReports != null && a.SubActionReports.Count > 0)
                {
                    FillTreeViewItem(currentLevel, a);
                }
            }
        }


        private void lvTeststeps_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            spScreenShots.Children.Clear();
            if (e.NewValue is TreeViewItem)
            {
                TreeViewItem item = e.NewValue as TreeViewItem;
                if (item.Tag is ActionReport step)
                {
                    txtInformation.Text = step.Log;
                    lbContextValues.Items.Clear();
                    if (step.ContextValues != null)
                    {
                        foreach (var c in step.ContextValues)
                        {
                            lbContextValues.Items.Add(c);
                        }
                    }
                    if (lbContextValues.Items.Count == 0)
                    {
                        lbContextValues.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        lbContextValues.Visibility = System.Windows.Visibility.Visible;
                    }

                    if (step.Screenshots?.Count > 0)
                    {
                        foreach (var data in step.Screenshots)
                        {
                            var image = new BitmapImage();

                            using (var mem = new MemoryStream(data.Data))
                            {
                                mem.Position = 0;
                                image.BeginInit();
                                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                                image.CacheOption = BitmapCacheOption.OnLoad;
                                image.UriSource = null;
                                image.StreamSource = mem;
                                image.EndInit();
                            }
                            image.Freeze();
                            Image img = new Image() { Source = image, Margin = new Thickness(10), MaxHeight = 300, MaxWidth = 400 };
                            img.ToolTip = data.Name;
                            img.MouseRightButtonDown += delegate
                            {
                                var screenshot = App.ConvertToBitmap(img, 96);
                                App.SaveImageToClipboard(screenshot);
                                ShowMessageBox("The screenshot has been saved to clipboard", "Screenshot", MessageBoxButton.OK, MessageBoxImage.Information);
                            };
                            img.MouseLeftButtonDown += Img_MouseLeftButtonDown;
                            spScreenShots.Children.Add(img);
                        }
                    }
                    else
                    {
                        spScreenShots.Children.Clear();
                    }
                }
            }
            else
            {
                txtInformation.Text = string.Empty;
                lbContextValues.Items.Clear();
            }
        }

        private void ShowXmlReport_Click(object sender, RoutedEventArgs e)
        {
            WindowViewXML xml = new WindowViewXML(xmlReport);
            xml.ShowDialog();
        }
        private bool Reseting = false;

        private void OpenReport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                CheckFileExists = true,
                Filter = "WebEngine Report (*.xml)|*.xml",
                Title = "Open WebEngine Report",
                CheckPathExists = true,
            };
            var result = ofd.ShowDialog(this);
            if (result.HasValue && result.Value)
            {
                App.LogFile = ofd.FileName;

                this.Reseting = true;
                txFilter.Text = string.Empty;
                cbFailed.IsChecked = false;
                this.Reseting = false;

                Load();
            }
        }

        private void Img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowShowImage showimage = new WindowShowImage();
            showimage.image.Source = (sender as Image).Source.Clone();
            showimage.ShowDialog();
            showimage.Close();
        }

        private void BtnCopyClipboard_Click(object sender, RoutedEventArgs e)
        {
            var tag = ((Control)sender).Tag?.ToString();
            if (tag != null)
            {
                Clipboard.SetText(tag);
                ShowToastNotification($"'{tag}' copied to clipboard", NotificationLevel.Information, null, 2);
            }
        }

        private void TextBlock_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var tag = ((TextBlock)sender).Text;
            if (tag != null)
            {
                Clipboard.SetText(tag);
                ShowToastNotification($"'{tag}' copied to clipboard", NotificationLevel.Information, null, 2);
            }
        }

        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            WindowAbout wa = new WindowAbout();
            wa.ShowDialog();
            wa.Close();
        }

        private void TxFilter_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.IsInitialized)
            {
                return;
            }
            App.Filter = txFilter.Text;
            if (this.Reseting)
            {
                return;
            }
            ApplyFilter();
        }

        private void CheckedChanged()
        {
            if (!this.IsInitialized)
            {
                return;
            }
            App.FailedTestOnly = cbFailed.IsChecked.HasValue && cbFailed.IsChecked.Value;
            if (this.Reseting)
            {
                return;
            }
            ApplyFilter();
        }

        private void CbFailed_OnChecked(object sender, RoutedEventArgs e)
        {
            this.CheckedChanged();
        }

        private void cbFailed_Unchecked(object sender, RoutedEventArgs e)
        {
            this.CheckedChanged();
        }

        private void OpenAccessibilityReport(byte[] data)
        {
            //extract data as zip stream to temp folder
            using (var stream = new MemoryStream(data))
            {
                ZipArchive archive = new ZipArchive(stream);
                var tempFolder = Path.Combine(Path.GetTempPath(), "AccessibilityReport");
                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, true);
                }
                Directory.CreateDirectory(tempFolder);
                archive.ExtractToDirectory(tempFolder);
                //open the index.html
                var indexFile = Directory.GetFiles(tempFolder, "index.html", SearchOption.AllDirectories).FirstOrDefault();
                if (indexFile != null)
                {
                    btnViewInBrowser.Tag = indexFile;
                    webViewAccessibility.Source = new Uri(indexFile);

                }
                else
                {
                    ShowMessageBox("Error", "Unable to find the index.html file in the accessibility report", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            webViewAccessibility.GoBack();
        }

        private void btnViewInBrowser_Click(object sender, RoutedEventArgs e)
        {
            if (btnViewInBrowser.Tag != null)
            {
                ProcessStartInfo psi = new ProcessStartInfo(btnViewInBrowser.Tag.ToString()) { UseShellExecute = true };
                Process.Start(psi);
            }
        }

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length >= 1)
                {
                    App.LogFile = files[0];
                    this.Reseting = true;
                    txFilter.Text = string.Empty;
                    cbFailed.IsChecked = false;
                    this.Reseting = false;
                    Load();
                }
            }
        }
    }
}
