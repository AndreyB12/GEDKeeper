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

using GKCommon.GEDCOM;
using GKCore.Types;

namespace GKCore.Interfaces
{
    public interface INamesTable
    {
        NameEntry AddName(string name);
        NameEntry FindName(string name);
        string GetPatronymicByName(string name, GEDCOMSex sex);
        string GetNameByPatronymic(string patronymic);
        GEDCOMSex GetSexByName(string name);
        void SetName(string name, string patronymic, GEDCOMSex sex);
        void SetNameSex(string name, GEDCOMSex sex);
        void ImportNames(GEDCOMIndividualRecord iRec);
    }
}
