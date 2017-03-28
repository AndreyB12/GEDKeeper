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
using System.Windows.Forms;
using GKCommon.GEDCOM;
using GKCore;
using GKCore.Interfaces;

namespace GKUI.Dialogs
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class FilePropertiesDlg : Form, IBaseEditor
    {
        private readonly IBaseWindow fBase;

        public IBaseWindow Base
        {
            get { return fBase; }
        }

        public FilePropertiesDlg(IBaseWindow baseWin)
        {
            InitializeComponent();

            btnAccept.Image = GKResources.iBtnAccept;
            btnCancel.Image = GKResources.iBtnCancel;
            btnLangEdit.Image = GKResources.iRecEdit;

            fBase = baseWin;

            UpdateControls();

            // SetLang()
            Text = LangMan.LS(LSID.LSID_MIFileProperties);
            btnAccept.Text = LangMan.LS(LSID.LSID_DlgAccept);
            btnCancel.Text = LangMan.LS(LSID.LSID_DlgCancel);
            pageAuthor.Text = LangMan.LS(LSID.LSID_Author);
            lblName.Text = LangMan.LS(LSID.LSID_Name);
            lblAddress.Text = LangMan.LS(LSID.LSID_Address);
            lblTelephone.Text = LangMan.LS(LSID.LSID_Telephone);
            pageOther.Text = LangMan.LS(LSID.LSID_Other);
            lvRecordStats.Columns[0].Text = LangMan.LS(LSID.LSID_RM_Records);
            lblLanguage.Text = LangMan.LS(LSID.LSID_Language);
        }

        private void UpdateControls()
        {
            txtLanguage.Text = fBase.Tree.Header.Language.StringValue;

            GEDCOMSubmitterRecord submitter = fBase.Tree.GetSubmitter();
            txtName.Text = submitter.Name.FullName;
            txtAddress.Text = submitter.Address.Address.Text;

            if (submitter.Address.PhoneNumbers.Count > 0) {
                txtTel.Text = submitter.Address.PhoneNumbers[0].StringValue;
            }

            UpdateStats();
        }

        private void UpdateStats()
        {
            int[] stats = fBase.Tree.GetRecordStats();

            lvRecordStats.Items.Clear();
            for (int i = 1; i < stats.Length; i++)
            {
                ListViewItem item = lvRecordStats.Items.Add(LangMan.LS(GKData.RecordTypes[i]));
                item.SubItems.Add(stats[i].ToString());
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            try
            {
                fBase.Tree.Header.Language.ParseString(txtLanguage.Text);

                GEDCOMSubmitterRecord submitter = fBase.Tree.GetSubmitter();
                submitter.Name.StringValue = txtName.Text;
                submitter.Address.SetAddressArray(txtAddress.Lines);

                if (submitter.Address.PhoneNumbers.Count > 0) {
                    submitter.Address.PhoneNumbers[0].StringValue = txtTel.Text;
                } else {
                    submitter.Address.AddPhoneNumber(txtTel.Text);
                }

                submitter.ChangeDate.ChangeDateTime = DateTime.Now;
                fBase.Modified = true;
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                fBase.Host.LogWrite("FilePropertiesDlg.btnAccept_Click(): " + ex.Message);
                DialogResult = DialogResult.None;
            }
        }

        private void btnLangEdit_Click(object sender, EventArgs e)
        {
            using (var dlg = new LanguageEditDlg()) {
                dlg.LanguageID = fBase.Tree.Header.Language.Value;

                if (dlg.ShowDialog() == DialogResult.OK) {
                    // Assignment in control, instead of the header's property to work Cancel.
                    txtLanguage.Text = GEDCOMLanguage.GetLangName(dlg.LanguageID);
                }
            }
        }
    }
}
