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

using GKCommon;
using GKCommon.GEDCOM;

namespace GKCore.Stats
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class CompositeItem
    {
        private float CommonSum;
        private float MaleSum;
        private float FemaleSum;

        private int CommonCount;
        private int MaleCount;
        private int FemaleCount;

        public double CommonVal { get { return SysUtils.SafeDiv(CommonSum, CommonCount); } }
        public double MaleVal { get { return SysUtils.SafeDiv(MaleSum, MaleCount); } }
        public double FemaleVal { get { return SysUtils.SafeDiv(FemaleSum, FemaleCount); } }

        public CompositeItem()
        {
            CommonSum = 0;
            MaleSum = 0;
            FemaleSum = 0;

            CommonCount = 0;
            MaleCount = 0;
            FemaleCount = 0;
        }

        public void TakeVal(float val, GEDCOMSex sex, bool ignoreZero)
        {
            if (val == 0.0f && ignoreZero) return;

            CommonSum += val;
            CommonCount++;
            
            switch (sex) {
                case GEDCOMSex.svFemale:
                    FemaleSum += val;
                    FemaleCount++;
                    break;

                case GEDCOMSex.svMale:
                    MaleSum += val;
                    MaleCount++;
                    break;
            }
        }

        public void TakeVal(string val, GEDCOMSex sex, bool ignoreZero)
        {
            int tmp;
            if (int.TryParse(val, out tmp))
            {
                TakeVal(tmp, sex, ignoreZero);
            }
        }
    }
}
