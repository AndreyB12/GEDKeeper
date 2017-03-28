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
    public enum ResearchColumnType
    {
        ctName,
        ctPriority,
        ctStatus,
        ctStartDate,
        ctStopDate,
        ctPercent,
        ctChangeDate
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class ResearchListColumns : ListColumns
    {
        protected override void InitColumnStatics()
        {
            AddColumn(LSID.LSID_Title, DataType.dtString, 300, true);
            AddColumn(LSID.LSID_Priority, DataType.dtString, 90, true);
            AddColumn(LSID.LSID_Status, DataType.dtString, 90, true);
            AddColumn(LSID.LSID_StartDate, DataType.dtString, 90, true);
            AddColumn(LSID.LSID_StopDate, DataType.dtString, 90, true);
            AddColumn(LSID.LSID_Percent, DataType.dtInteger, 90, true);
            AddColumn(LSID.LSID_Changed, DataType.dtDateTime, 150, true);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class ResearchListMan : ListManager
    {
        private GEDCOMResearchRecord fRec;

        public ResearchListMan(GEDCOMTree tree) : base(tree, new ResearchListColumns())
        {
        }

        public override bool CheckFilter(ShieldState shieldState)
        {
            bool res = (QuickFilter == "*" || IsMatchesMask(fRec.ResearchName, QuickFilter));

            res = res && CheckCommonFilter();

            return res;
        }

        public override void Fetch(GEDCOMRecord aRec)
        {
            fRec = (aRec as GEDCOMResearchRecord);
        }

        protected override object GetColumnValueEx(int colType, int colSubtype, bool isVisible)
        {
            object result = null;
            switch ((ResearchColumnType)colType)
            {
                case ResearchColumnType.ctName:
                    result = fRec.ResearchName;
                    break;

                case ResearchColumnType.ctPriority:
                    result = LangMan.LS(GKData.PriorityNames[(int)fRec.Priority]);
                    break;

                case ResearchColumnType.ctStatus:
                    result = LangMan.LS(GKData.StatusNames[(int)fRec.Status]);
                    break;

                case ResearchColumnType.ctStartDate:
                    result = GetDateValue(fRec.StartDate, isVisible);
                    break;

                case ResearchColumnType.ctStopDate:
                    result = GetDateValue(fRec.StopDate, isVisible);
                    break;

                case ResearchColumnType.ctPercent:
                    result = fRec.Percent;
                    break;

                case ResearchColumnType.ctChangeDate:
                    result = fRec.ChangeDate.ChangeDateTime;
                    break;
            }
            return result;
        }
    }
}
