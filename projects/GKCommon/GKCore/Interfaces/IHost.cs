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

using System.Windows.Forms;
using GKCore.Types;

namespace GKCore.Interfaces
{
    public interface IHost : ILocalization
    {
        INamesTable NamesTable { get; }

        IBaseWindow GetCurrentFile(bool extMode = false);
        IWorkWindow GetWorkWindow();

        string GetUserFilesPath(string filePath);
        IBaseWindow CreateBase(string fileName);
        IBaseWindow FindBase(string fileName);
        void BaseChanged(IBaseWindow baseWin);
        void BaseClosed(IBaseWindow baseWin);
        void BaseRenamed(IBaseWindow baseWin, string oldName, string newName);
        void NotifyRecord(IBaseWindow baseWin, object record, RecordAction action);

        string GetAppDataPath();

        void LogWrite(string msg);

        bool IsWidgetActive(IWidget widget);
        void WidgetShow(IWidget widget);
        void WidgetClose(IWidget widget);

        void ShowMDI(Form form);

        ILangMan CreateLangMan(object sender);
        void LoadLanguage(int langCode);
        void UpdateNavControls();
        void UpdateControls(bool forceDeactivate);
        void ShowHelpTopic(string topic);
        void EnableWindow(Form form, bool value);
        void Restore();

        bool IsUnix();
        void ShowWarning(string msg);
    }
}
