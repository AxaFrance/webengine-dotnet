// Copyright (c) 2016-2026 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Drawing;
using System.Windows.Forms;

namespace AxaFrance.WebEngine.ExcelAddin
{
    /// <summary>
    /// Shared visual style for the add-in dialogs.
    /// </summary>
    internal static class UiTheme
    {
        internal static readonly Color Header = Color.SteelBlue;
        internal static readonly Color Border = Color.Gainsboro;
        internal static readonly Color Checked = Color.FromArgb(210, 232, 250);

        internal static void StyleForm(Form form, string text, Size clientSize)
        {
            form.Text = text;
            form.ClientSize = clientSize;
            form.BackColor = Color.White;
            form.FormBorderStyle = FormBorderStyle.FixedSingle;
            form.StartPosition = FormStartPosition.CenterParent;
            form.MaximizeBox = false;
            form.MinimizeBox = false;
            form.ShowInTaskbar = false;
            form.AutoScaleMode = AutoScaleMode.Dpi;
        }

        internal static Label CreateHeader(string text, int x, int y, int width)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, 22),
                ForeColor = Header,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            };
        }

        internal static Label CreateFieldLabel(string text, int x, int y, int width = 130)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, 20),
                ForeColor = Header,
            };
        }

        internal static Button CreateButton(string text, int x, int y, int width = 90, int height = 30)
        {
            var button = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = Color.WhiteSmoke,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false,
            };
            button.FlatAppearance.BorderColor = Border;
            button.FlatAppearance.BorderSize = 2;
            button.FlatAppearance.MouseOverBackColor = Color.LightSkyBlue;
            button.FlatAppearance.MouseDownBackColor = Border;
            return button;
        }

        internal static RadioButton CreateBrowserTile(string text, Bitmap? image, int x, int y)
        {
            var tile = new RadioButton
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(128, 116),
                Appearance = Appearance.Button,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.BottomCenter,
                Image = image,
                ImageAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.ImageAboveText,
                BackColor = Color.White,
                UseVisualStyleBackColor = false,
                AutoEllipsis = true,
                Padding = new Padding(4),
            };
            tile.FlatAppearance.BorderColor = Border;
            tile.FlatAppearance.BorderSize = 1;
            tile.FlatAppearance.CheckedBackColor = Checked;
            tile.FlatAppearance.MouseOverBackColor = Color.FromArgb(238, 247, 255);
            tile.FlatAppearance.MouseDownBackColor = Checked;
            return tile;
        }
    }
}
