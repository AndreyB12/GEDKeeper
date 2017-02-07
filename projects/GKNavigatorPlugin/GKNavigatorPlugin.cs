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
using System.Reflection;
using System.Runtime.InteropServices;

using GKCommon;
using GKCore.Interfaces;
using GKCore.Types;

[assembly: AssemblyTitle("GKNavigatorPlugin")]
[assembly: AssemblyDescription("GEDKeeper Navigator plugin")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("GEDKeeper")]
[assembly: AssemblyCopyright("Copyright © 2016 by Sergey V. Zhdanovskih")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(false)]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

namespace GKNavigatorPlugin
{
    public enum PLS
    {
        /* 001 */ LSID_Navigator,
    }

    public sealed class Plugin : BaseObject, IPlugin, IWidget, ISubscriber
    {
        private string fDisplayName = "GKNavigatorPlugin";
        private IHost fHost;
        private ILangMan fLangMan;
        private NavigatorData fData;

        public string DisplayName { get { return fDisplayName; } }
        public IHost Host { get { return fHost; } }
        public ILangMan LangMan { get { return fLangMan; } }

        private NavigatorWidget frm;

        internal NavigatorData Data
        {
            get { return this.fData; }
        }

        public Plugin()
        {
            this.fData = new NavigatorData();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (frm != null) {
                    frm.Dispose();
                    frm = null;
                }
            }
            base.Dispose(disposing);
        }

        #region IPlugin support

        public void Execute()
        {
            if (!this.fHost.IsWidgetActive(this)) {
                frm = new NavigatorWidget(this);
                frm.Show();
            } else {
                frm.Close();
                frm = null;
            }
        }

        public void OnHostClosing(ref bool cancelClosing) {}
        public void OnHostActivate() {}
        public void OnHostDeactivate() {}

        public void OnLanguageChange()
        {
            try
            {
                this.fLangMan = this.fHost.CreateLangMan(this);
                this.fDisplayName = this.fLangMan.LS(PLS.LSID_Navigator);

                if (frm != null) frm.SetLang();
            }
            catch (Exception ex)
            {
                fHost.LogWrite("GKNavigatorPlugin.OnLanguageChange(): " + ex.Message);
            }
        }

        public bool Startup(IHost host)
        {
            bool result = true;
            try
            {
                this.fHost = host;
                // Implement any startup code here
            }
            catch (Exception ex)
            {
                fHost.LogWrite("GKNavigatorPlugin.Startup(): " + ex.Message);
                result = false;
            }
            return result;
        }

        public bool Shutdown()
        {
            bool result = true;
            try
            {
                // Implement any shutdown code here
            }
            catch (Exception ex)
            {
                fHost.LogWrite("GKNavigatorPlugin.Shutdown(): " + ex.Message);
                result = false;
            }
            return result;
        }

        #endregion

        #region IWidget support

        void IWidget.WidgetInit(IHost host) {}

        void IWidget.BaseChanged(IBaseWindow baseWin)
        {
            if (frm != null) {
                frm.BaseChanged(baseWin);
            }
        }

        void IWidget.BaseClosed(IBaseWindow baseWin)
        {
            if (frm != null) {
                frm.BaseClosed(baseWin);
            }
        }

        void IWidget.BaseRenamed(IBaseWindow baseWin, string oldName, string newName)
        {
            this.fData.RenameBase(oldName, newName);
        }

        void IWidget.WidgetEnable() {}

        #endregion

        #region ISubscriber support

        public void NotifyRecord(IBaseWindow baseWin, object record, RecordAction action)
        {
            if (baseWin == null || record == null) return;

            string baseName = baseWin.Context.Tree.FileName;
            fData[baseName].NotifyRecord(baseWin, record, action);
        }

        #endregion
    }
}
