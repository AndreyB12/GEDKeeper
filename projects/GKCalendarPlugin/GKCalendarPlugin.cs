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

[assembly: AssemblyTitle("GKCalendarPlugin")]
[assembly: AssemblyDescription("GEDKeeper Calendar plugin")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("GEDKeeper")]
[assembly: AssemblyCopyright("Copyright © 2014 by Sergey V. Zhdanovskih")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(false)]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

namespace GKCalendarPlugin
{
    public enum PLS
    {
        LSID_MICalendar,
        LSID_Date,
        LSID_Cal_Gregorian,
        LSID_Cal_Julian,
        LSID_Cal_Hebrew,
        LSID_Cal_Islamic,
        LSID_Cal_Persian,
        LSID_Cal_Indian,
        LSID_Cal_Bahai,

        LSID_BahaiMonths,
        LSID_BahaiWeekdays,
        LSID_ClassicMonths,
        LSID_ClassicWeekdays,
        LSID_HebrewMonths,
        LSID_HebrewWeekdays,
        LSID_IndianCivilMonths,
        LSID_IndianCivilWeekdays,
        LSID_IslamicMonths,
        LSID_IslamicWeekdays,
        LSID_PersianMonths,
        LSID_PersianWeekdays,
        LSID_BahaiCycles
    }

    public sealed class Plugin : BaseObject, IPlugin, IWidget
    {
        private string fDisplayName = "GKCalendarPlugin";
        private IHost fHost;
        private ILangMan fLangMan;

        public string DisplayName { get { return fDisplayName; } }
        public IHost Host { get { return fHost; } }
        public ILangMan LangMan { get { return fLangMan; } }

        private CalendarWidget fForm;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (fForm != null) fForm.Dispose();
            }
            base.Dispose(disposing);
        }

        public void Execute()
        {
            if (!fHost.IsWidgetActive(this)) {
                fForm = new CalendarWidget(this);
                fForm.Show();
            } else {
                fForm.Close();
            }
        }

        public void OnHostClosing(ref bool cancelClosing) {}
        public void OnHostActivate() {}
        public void OnHostDeactivate() {}

        public void OnLanguageChange()
        {
            try
            {
                fLangMan = fHost.CreateLangMan(this);
                fDisplayName = fLangMan.LS(PLS.LSID_MICalendar);

                CalendarData.BahaiMonths = CalendarData.InitNames(fLangMan.LS(PLS.LSID_BahaiMonths));
                CalendarData.BahaiWeekdays = CalendarData.InitNames(fLangMan.LS(PLS.LSID_BahaiWeekdays));
                CalendarData.ClassicMonths = CalendarData.InitNames(fLangMan.LS(PLS.LSID_ClassicMonths));
                CalendarData.ClassicWeekdays = CalendarData.InitNames(fLangMan.LS(PLS.LSID_ClassicWeekdays));
                CalendarData.HebrewMonths = CalendarData.InitNames(fLangMan.LS(PLS.LSID_HebrewMonths));
                CalendarData.HebrewWeekdays = CalendarData.InitNames(fLangMan.LS(PLS.LSID_HebrewWeekdays));
                CalendarData.IndianCivilMonths = CalendarData.InitNames(fLangMan.LS(PLS.LSID_IndianCivilMonths));
                CalendarData.IndianCivilWeekdays = CalendarData.InitNames(fLangMan.LS(PLS.LSID_IndianCivilWeekdays));
                CalendarData.IslamicMonths = CalendarData.InitNames(fLangMan.LS(PLS.LSID_IslamicMonths));
                CalendarData.IslamicWeekdays = CalendarData.InitNames(fLangMan.LS(PLS.LSID_IslamicWeekdays));
                CalendarData.PersianMonths = CalendarData.InitNames(fLangMan.LS(PLS.LSID_PersianMonths));
                CalendarData.PersianWeekdays = CalendarData.InitNames(fLangMan.LS(PLS.LSID_PersianWeekdays));

                if (fForm != null) fForm.SetLang();
            }
            catch (Exception ex)
            {
                fHost.LogWrite("GKCalendarPlugin.OnLanguageChange(): " + ex.Message);
            }
        }

        public bool Startup(IHost host)
        {
            bool result = true;
            try
            {
                fHost = host;
            }
            catch (Exception ex)
            {
                fHost.LogWrite("GKCalendarPlugin.Startup(): " + ex.Message);
                result = false;
            }
            return result;
        }

        public bool Shutdown()
        {
            bool result = true;
            try
            {
            }
            catch (Exception ex)
            {
                fHost.LogWrite("GKCalendarPlugin.Shutdown(): " + ex.Message);
                result = false;
            }
            return result;
        }

        #region IWidget common

        void IWidget.WidgetInit(IHost host) {}
        void IWidget.BaseChanged(IBaseWindow baseWin) {}
        void IWidget.BaseClosed(IBaseWindow baseWin) {}
        void IWidget.BaseRenamed(IBaseWindow baseWin, string oldName, string newName) {}
        void IWidget.WidgetEnable() {}

        #endregion
    }
}
