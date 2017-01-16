﻿/*
 *  "GEDKeeper", the personal genealogical database editor.
 *  Copyright (C) 2009-2016 by Serg V. Zhdanovskih (aka Alchemist, aka Norseman).
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
    public enum TaskColumnType
    {
        tctGoal,
        tctPriority,
        tctStartDate,
        tctStopDate,
        tctChangeDate
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class TaskListColumns : ListColumns
    {
        protected override void InitColumnStatics()
        {
            this.AddStatic(LSID.LSID_Goal, DataType.dtString, 300, true);
            this.AddStatic(LSID.LSID_Priority, DataType.dtString, 90, true);
            this.AddStatic(LSID.LSID_StartDate, DataType.dtString, 90, true);
            this.AddStatic(LSID.LSID_StopDate, DataType.dtString, 90, true);
            this.AddStatic(LSID.LSID_Changed, DataType.dtDateTime, 150, true);
        }

        public TaskListColumns() : base()
        {
            InitData(typeof(TaskColumnType));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class TaskListMan : ListManager
    {
        private GEDCOMTaskRecord fRec;

        public override bool CheckFilter(ShieldState shieldState)
        {
            bool res = (this.QuickFilter == "*" || IsMatchesMask(GKUtils.GetTaskGoalStr(this.fRec), this.QuickFilter));

            res = res && base.CheckCommonFilter();

            return res;
        }

        public override void Fetch(GEDCOMRecord aRec)
        {
            this.fRec = (aRec as GEDCOMTaskRecord);
        }

        protected override object GetColumnValueEx(int colType, int colSubtype, bool isVisible)
        {
            object result = null;
            switch (colType) {
                case 0:
                    result = GKUtils.GetTaskGoalStr(this.fRec);
                    break;
                case 1:
                    result = LangMan.LS(GKData.PriorityNames[(int)this.fRec.Priority]);
                    break;
                case 2:
                    result = GetDateValue(this.fRec.StartDate, isVisible);
                    break;
                case 3:
                    result = GetDateValue(this.fRec.StopDate, isVisible);
                    break;
                case 4:
                    result = this.fRec.ChangeDate.ChangeDateTime;
                    break;
            }
            return result;
        }

        public TaskListMan(GEDCOMTree tree) : base(tree, new TaskListColumns())
        {
        }
    }
}
