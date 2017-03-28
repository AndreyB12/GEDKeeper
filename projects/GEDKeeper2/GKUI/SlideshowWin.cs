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

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using GKCommon.Controls;
using GKCommon.GEDCOM;
using GKCore;
using GKCore.Interfaces;
using GKCore.Types;

namespace GKUI
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SlideshowWin : Form, ILocalization, IWorkWindow
    {
        private readonly IBaseWindow fBase;
        private readonly List<GEDCOMFileReferenceWithTitle> fFileRefs;
        private readonly ImageBox fImageCtl;

        private int fCurrentIndex;
        private string fCurrentText;

        public SlideshowWin(IBaseWindow baseWin)
        {
            InitializeComponent();

            tbStart.Image = GKResources.iStart;
            tbPrev.Image = GKResources.iLeft1;
            tbNext.Image = GKResources.iRight1;

            SuspendLayout();
            fImageCtl = new ImageBox();
            fImageCtl.Dock = DockStyle.Fill;
            fImageCtl.Location = new Point(0, 0);
            fImageCtl.Size = new Size(100, 100);
            Controls.Add(fImageCtl);
            Controls.SetChildIndex(fImageCtl, 0);
            ResumeLayout(false);

            fImageCtl.BackColor = SystemColors.ControlDark;
            fImageCtl.Margin = new Padding(4);
            fImageCtl.ImageBorderStyle = ImageBoxBorderStyle.FixedSingleGlowShadow;
            fImageCtl.ImageBorderColor = Color.AliceBlue;
            fImageCtl.SelectionMode = ImageBoxSelectionMode.Zoom;

            WindowState = FormWindowState.Maximized;

            SetLang();

            fBase = baseWin;
            fFileRefs = new List<GEDCOMFileReferenceWithTitle>();
            fCurrentIndex = -1;

            LoadList();
        }

        private void SlideshowWin_Load(object sender, System.EventArgs e)
        {
            if (fFileRefs.Count > 0) {
                fCurrentIndex = 0;
                SetFileRef();
            } else {
                UpdateControls();
            }
        }

        private void SlideshowWin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) Close();
        }

        private void LoadList()
        {
            GEDCOMRecord record;
            var enumerator = fBase.Tree.GetEnumerator(GEDCOMRecordType.rtMultimedia);
            while (enumerator.MoveNext(out record)) {
                GEDCOMMultimediaRecord mediaRec = (GEDCOMMultimediaRecord)record;
                GEDCOMFileReferenceWithTitle fileRef = mediaRec.FileReferences[0];

                MultimediaKind mmKind = GKUtils.GetMultimediaKind(fileRef.MultimediaFormat);
                if (mmKind == MultimediaKind.mkImage) {
                    fFileRefs.Add(fileRef);
                }
            }
        }

        public void SetLang()
        {
            Text = LangMan.LS(LSID.LSID_Slideshow);
            tbPrev.ToolTipText = LangMan.LS(LSID.LSID_PrevRec);
            tbNext.ToolTipText = LangMan.LS(LSID.LSID_NextRec);

            //fImageCtl.btnSizeToFit.Text = LangMan.LS(LSID.LSID_SizeToFit);
            //fImageCtl.btnZoomIn.Text = LangMan.LS(LSID.LSID_ZoomIn);
            //fImageCtl.btnZoomOut.Text = LangMan.LS(LSID.LSID_ZoomOut);
        }

        private void SetFileRef()
        {
            if (fCurrentIndex < 0 || fCurrentIndex >= fFileRefs.Count) return;

            GEDCOMFileReferenceWithTitle fileRef = fFileRefs[fCurrentIndex];

            fCurrentText = fileRef.Title;

            switch (fileRef.MultimediaFormat)
            {
                case GEDCOMMultimediaFormat.mfBMP:
                case GEDCOMMultimediaFormat.mfGIF:
                case GEDCOMMultimediaFormat.mfJPG:
                case GEDCOMMultimediaFormat.mfPCX:
                case GEDCOMMultimediaFormat.mfTIF:
                case GEDCOMMultimediaFormat.mfTGA:
                case GEDCOMMultimediaFormat.mfPNG:
                    {
                        Image img = fBase.Context.LoadMediaImage(fileRef, false);
                        if (img != null) {
                            fImageCtl.Image = img;
                            fImageCtl.ZoomToFit();
                        }
                        break;
                    }
            }

            UpdateControls();
        }

        private void tsbStart_Click(object sender, System.EventArgs e)
        {
            if (tbStart.Text == LangMan.LS(LSID.LSID_Start)) {
                tbStart.Text = LangMan.LS(LSID.LSID_Stop);
                tbStart.Image = GKResources.iStop;
                timer1.Enabled = true;
            } else {
                tbStart.Text = LangMan.LS(LSID.LSID_Start);
                tbStart.Image = GKResources.iStart;
                timer1.Enabled = false;
            }
        }

        private void tsbPrev_Click(object sender, System.EventArgs e)
        {
            fCurrentIndex--;
            SetFileRef();
        }

        private void tsbNext_Click(object sender, System.EventArgs e)
        {
            fCurrentIndex++;
            SetFileRef();
        }

        private void UpdateControls()
        {
            tbStart.Enabled = (fFileRefs.Count > 0);
            tbPrev.Enabled = (fCurrentIndex > 0);
            tbNext.Enabled = (fCurrentIndex < fFileRefs.Count - 1);

            MainWin.Instance.UpdateControls(false);
        }

        private void Timer1Tick(object sender, System.EventArgs e)
        {
            if (fCurrentIndex < fFileRefs.Count - 1) {
                fCurrentIndex++;
            } else {
                fCurrentIndex = 0;
            }

            SetFileRef();
        }

        #region IWorkWindow implementation

        public string GetStatusString()
        {
            return string.Format("{0} / {1} [{2}]", fCurrentIndex + 1, fFileRefs.Count, fCurrentText);
        }

        public void UpdateView()
        {
            // dummy
        }

        public void NavNext()
        {
        }

        public void NavPrev()
        {
        }

        public bool NavCanBackward()
        {
            return false;
        }

        public bool NavCanForward()
        {
            return false;
        }

        public bool AllowQuickSearch()
        {
            return false;
        }

        public IList<ISearchResult> FindAll(string searchPattern)
        {
            return null;
        }

        public void SelectByRec(GEDCOMIndividualRecord iRec)
        {
        }

        public void QuickSearch()
        {
        }

        public bool AllowFilter()
        {
            return false;
        }

        public void SetFilter()
        {
        }

        #endregion
    }
}
