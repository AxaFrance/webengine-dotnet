// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Microsoft.Win32;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;


namespace AxaFrance.WebEngine.ReportViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string LogFile { get; set; }
        public static string Filter { get; set; }
        public static bool FailedTestOnly { get; set; }

        /// <summary>Raised on the UI thread whenever the OS theme changes.</summary>
        public static event Action<bool> ThemeChanged;


        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.StartupUri = new Uri("MainWindow.xaml", UriKind.RelativeOrAbsolute);
            if (e.Args.Length > 0)
            {
                LogFile = e.Args[0];
            }
            LoadSyntaxConfig();
            ApplyTheme(IsWindowsLightMode());
            StartThemeWatcher();
        }

        /// <summary>
        /// Reads the Windows 10/11 "AppsUseLightTheme" registry value.
        /// Returns true when the OS is in light mode, false for dark mode.
        /// </summary>
        public static bool IsWindowsLightMode()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                if (key?.GetValue("AppsUseLightTheme") is int value)
                    return value == 1;
            }
            catch { }
            return false; // default to dark if unreadable
        }

        /// <summary>
        /// Swaps the application-level ResourceDictionary to match the requested theme.
        /// Can be called from any thread – marshals to the UI thread automatically.
        /// </summary>
        public static void ApplyTheme(bool lightMode)
        {
            Current.Dispatcher.Invoke(() =>
            {
                var themePath = lightMode
                    ? "Styles/LightTheme.xaml"
                    : "Styles/ModernTheme.xaml";

                var newDict = new ResourceDictionary
                {
                    Source = new Uri(themePath, UriKind.Relative)
                };

                var merged = Current.Resources.MergedDictionaries;
                // Remove previous theme dictionary (if any) and add the new one
                for (int i = merged.Count - 1; i >= 0; i--)
                {
                    var src = merged[i].Source?.OriginalString ?? string.Empty;
                    if (src.Contains("Theme.xaml", StringComparison.OrdinalIgnoreCase))
                    {
                        merged.RemoveAt(i);
                    }
                }
                merged.Add(newDict);
                ThemeChanged?.Invoke(lightMode);
            });
        }

        /// <summary>
        /// Subscribes to <see cref="SystemEvents.UserPreferenceChanged"/> so the theme
        /// reacts when the user toggles light/dark mode in Windows Settings without
        /// restarting the app. No extra NuGet package required – the event is part of
        /// the Windows Desktop SDK already referenced by every WPF project.
        /// </summary>
        private void StartThemeWatcher()
        {
            SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
        }

        private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            // General fires for color/theme changes; Policy fires on managed-device changes.
            if (e.Category == UserPreferenceCategory.General)
                ApplyTheme(IsWindowsLightMode());
        }

        protected override void OnExit(ExitEventArgs e)
        {
            SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
            base.OnExit(e);
        }

        private static void LoadSyntaxConfig()
        {
            RegisterHighlighting("AxaFrance.WebEngine.ReportViewer.XmlDark.xshd", "XML_DARK");
            RegisterHighlighting("AxaFrance.WebEngine.ReportViewer.XmlLight.xshd", "XML_LIGHT");
        }

        private static void RegisterHighlighting(string resourceName, string highlightingName)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            using var reader = XmlReader.Create(stream);
            var definition = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            HighlightingManager.Instance.RegisterHighlighting(highlightingName, new[] { highlightingName }, definition);
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
