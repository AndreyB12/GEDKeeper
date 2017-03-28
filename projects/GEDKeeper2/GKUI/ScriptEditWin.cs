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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using GKCommon;
using GKCore;
using GKCore.Interfaces;

namespace GKUI
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class ScriptEditWin : Form, ILocalization
    {
        private readonly IBaseWindow fBase;

        private string fFileName;
        private bool fModified;

        public string FileName
        {
            get {
                return fFileName;
            }
            set {
                fFileName = value;
                SetTitle();
            }
        }

        public bool Modified
        {
            get {
                return fModified;
            }
            set {
                fModified = value;
                SetTitle();
            }
        }

        private bool CheckModified()
        {
            bool result = true;
            if (!Modified) return result;

            DialogResult dialogResult = MessageBox.Show(LangMan.LS(LSID.LSID_FileSaveQuery), GKData.APP_TITLE, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
            switch (dialogResult) {
                case DialogResult.Yes:
                    tbSaveScript_Click(this, null);
                    break;
                case DialogResult.No:
                    break;
                case DialogResult.Cancel:
                    result = false;
                    break;
            }

            return result;
        }

        private void SetTitle()
        {
            Text = Path.GetFileName(fFileName);
            if (fModified)
            {
                Text = @"* " + Text;
            }
        }

        private void tbNewScript_Click(object sender, EventArgs e)
        {
            if (!CheckModified()) return;

            txtScriptText.Clear();
            FileName = "unknown.lua";
            Modified = false;
        }

        private void tbLoadScript_Click(object sender, EventArgs e)
        {
            if (!CheckModified()) return;

            string fileName = UIHelper.GetOpenFile("", "", LangMan.LS(LSID.LSID_ScriptsFilter), 1, GKData.LUA_EXT);
            if (string.IsNullOrEmpty(fileName)) return;

            using (StreamReader strd = new StreamReader(File.OpenRead(fileName), Encoding.UTF8))
            {
                txtScriptText.Text = strd.ReadToEnd();
                FileName = fileName;
                Modified = false;
                strd.Close();
            }
        }

        private void tbSaveScript_Click(object sender, EventArgs e)
        {
            string fileName = UIHelper.GetSaveFile("", "", LangMan.LS(LSID.LSID_ScriptsFilter), 1, GKData.LUA_EXT, FileName);
            if (string.IsNullOrEmpty(fileName)) return;

            using (StreamWriter strd = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                strd.Write(txtScriptText.Text);
                FileName = fileName;
                Modified = false;
                strd.Close();
            }
        }

        private void tbRun_Click(object sender, EventArgs e)
        {
            try
            {
                txtDebugOutput.Clear();
                using (ScriptEngine scrEngine = new ScriptEngine()) {
                    scrEngine.lua_run(txtScriptText.Text, fBase, txtDebugOutput);
                }
            }
            catch (Exception ex)
            {
                fBase.Host.LogWrite("ScriptEditWin.Run(): " + ex.Message);
                fBase.Host.LogWrite("ScriptEditWin.Run(): " + ex.StackTrace);
            }
        }

        private void ScriptEditWin_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = !CheckModified();
        }

        private void mmScriptText_TextChanged(object sender, EventArgs e)
        {
            Modified = true;
        }

        public ScriptEditWin(IBaseWindow baseWin)
        {
            InitializeComponent();

            tbNewScript.Image = (Image)MainWin.ResourceManager.GetObjectEx("iCreateNew");
            tbLoadScript.Image = (Image)MainWin.ResourceManager.GetObjectEx("iLoad");
            tbSaveScript.Image = (Image)MainWin.ResourceManager.GetObjectEx("iSave");
            tbRun.Image = (Image)MainWin.ResourceManager.GetObjectEx("iStart");

            fBase = baseWin;

            txtScriptText.TextChanged += mmScriptText_TextChanged;
            
            tbNewScript_Click(this, null);

            SetLang();
        }

        public void SetLang()
        {
            tbNewScript.ToolTipText = LangMan.LS(LSID.LSID_NewScriptTip);
            tbLoadScript.ToolTipText = LangMan.LS(LSID.LSID_LoadScriptTip);
            tbSaveScript.ToolTipText = LangMan.LS(LSID.LSID_SaveScriptTip);
            tbRun.ToolTipText = LangMan.LS(LSID.LSID_RunScriptTip);
        }

        private void ScriptEditWin_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Close();
                    break;
            }
        }
    }
}
