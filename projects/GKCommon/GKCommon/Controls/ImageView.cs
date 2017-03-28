﻿/*
 *  "GEDKeeper", the personal genealogical database editor.
 *  Copyright (C) 2009-2017 by Sergey V. Zhdanovskih.
 *
 *  This file is part of "GEDKeeper".
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GKCommon.Controls
{
    public partial class ImageView : UserControl
    {
        public bool ShowToolbar
        {
            get { return toolStrip.Visible; }
            set { toolStrip.Visible = value; }
        }

        public ImageBoxSelectionMode SelectionMode
        {
            get { return imageBox.SelectionMode; }
            set { imageBox.SelectionMode = value; }
        }

        public RectangleF SelectionRegion
        {
            get { return imageBox.SelectionRegion; }
            set { imageBox.SelectionRegion = value; }
        }


        public ImageView()
        {
            InitializeComponent();

            FillZoomLevels();
            imageBox.ImageBorderStyle = ImageBoxBorderStyle.FixedSingleGlowShadow;
            imageBox.ImageBorderColor = Color.AliceBlue;
            imageBox.SelectionMode = ImageBoxSelectionMode.Zoom;
            imageBox.AllowDoubleClick = false;
            imageBox.AllowZoom = true;
            imageBox.InterpolationMode = InterpolationMode.HighQualityBicubic;
        }

        public void OpenImage(Image image)
        {
            if (image == null) return;

            imageBox.BeginUpdate();
            imageBox.Image = image;
            imageBox.ZoomToFit();
            imageBox.EndUpdate();

            UpdateZoomLevels();
        }

        private void FillZoomLevels()
        {
            zoomLevelsToolStripComboBox.Items.Clear();

            foreach (int zoom in imageBox.ZoomLevels)
                zoomLevelsToolStripComboBox.Items.Add(string.Format("{0}%", zoom));
        }

        private void UpdateZoomLevels()
        {
            zoomLevelsToolStripComboBox.Text = string.Format("{0}%", imageBox.Zoom);
        }

        private void btnSizeToFit_Click(object sender, EventArgs e)
        {
            imageBox.ZoomToFit();
            UpdateZoomLevels();
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            imageBox.ZoomIn();
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            imageBox.ZoomOut();
        }

        private void imageBox_ZoomChanged(object sender, EventArgs e)
        {
            UpdateZoomLevels();
        }

        private void zoomLevelsToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int zoom = Convert.ToInt32(zoomLevelsToolStripComboBox.Text.Substring(0, zoomLevelsToolStripComboBox.Text.Length - 1));
            imageBox.Zoom = zoom;
        }

        protected override void Select(bool directed, bool forward)
        {
            base.Select(directed, forward);
            imageBox.Select();
        }
    }
}
