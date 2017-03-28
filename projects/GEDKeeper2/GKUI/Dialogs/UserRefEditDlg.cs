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
using GKCore.Types;

namespace GKUI.Dialogs
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class UserRefEditDlg : Form, IBaseEditor
    {
        private readonly IBaseWindow fBase;

        private GEDCOMUserReference fUserRef;

        public GEDCOMUserReference UserRef
        {
            get { return fUserRef; }
            set { SetUserRef(value); }
        }

        public IBaseWindow Base
        {
            get { return fBase; }
        }

        private void SetUserRef(GEDCOMUserReference value)
        {
            fUserRef = value;
            cmbRef.Text = fUserRef.StringValue;
            cmbRefType.Text = fUserRef.ReferenceType;
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            try
            {
                fUserRef.StringValue = cmbRef.Text;
                fUserRef.ReferenceType = cmbRefType.Text;
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                fBase.Host.LogWrite("UserRefEditDlg.btnAccept_Click(): " + ex.Message);
                DialogResult = DialogResult.None;
            }
        }

        public UserRefEditDlg(IBaseWindow baseWin)
        {
            InitializeComponent();

            btnAccept.Image = GKResources.iBtnAccept;
            btnCancel.Image = GKResources.iBtnCancel;

            fBase = baseWin;

            for (SpecialUserRef ur = SpecialUserRef.urCustom; ur <= SpecialUserRef.urLast; ur++)
            {
                cmbRef.Items.Add(GKData.SpecialUserRefs[(int)ur]);
            }

            // SetLang()
            btnAccept.Text = LangMan.LS(LSID.LSID_DlgAccept);
            btnCancel.Text = LangMan.LS(LSID.LSID_DlgCancel);
            Text = LangMan.LS(LSID.LSID_WinUserRefEdit);
            lblReference.Text = LangMan.LS(LSID.LSID_Reference);
            lblRefType.Text = LangMan.LS(LSID.LSID_Type);
        }
    }
}
