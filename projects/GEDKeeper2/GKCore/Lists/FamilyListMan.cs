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
using GKCore.Interfaces;
using GKCore.Types;

namespace GKCore.Lists
{
    /// <summary>
    /// 
    /// </summary>
    public enum FamilyColumnType
    {
        ctFamilyStr,
        ctMarriageDate,
        ctChangeDate
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class FamilyListColumns : ListColumns
    {
        protected override void InitColumnStatics()
        {
            AddColumn(LSID.LSID_Spouses, DataType.dtString, 300, true);
            AddColumn(LSID.LSID_MarriageDate, DataType.dtString, 100, true);
            AddColumn(LSID.LSID_Changed, DataType.dtDateTime, 150, true);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class FamilyListMan : ListManager
    {
        private GEDCOMFamilyRecord fRec;

        public FamilyListMan(GEDCOMTree tree) : base(tree, new FamilyListColumns())
        {
        }

        public override bool CheckFilter(ShieldState shieldState)
        {
            bool res = (GKUtils.IsRecordAccess(fRec.Restriction, shieldState)
                        && (QuickFilter == "*" || IsMatchesMask(GKUtils.GetFamilyString(fRec), QuickFilter)));

            res = res && CheckCommonFilter();

            return res;
        }

        public override void Fetch(GEDCOMRecord aRec)
        {
            fRec = (aRec as GEDCOMFamilyRecord);
        }

        protected override object GetColumnValueEx(int colType, int colSubtype, bool isVisible)
        {
            object result = null;
            switch ((FamilyColumnType)colType)
            {
                case FamilyColumnType.ctFamilyStr:
                    result = GKUtils.GetFamilyString(fRec);
                    break;

                case FamilyColumnType.ctMarriageDate:
                    result = GetDateValue(GKUtils.GetMarriageDate(fRec), isVisible);
                    break;

                case FamilyColumnType.ctChangeDate:
                    result = fRec.ChangeDate.ChangeDateTime;
                    break;
            }
            return result;
        }
    }
}
