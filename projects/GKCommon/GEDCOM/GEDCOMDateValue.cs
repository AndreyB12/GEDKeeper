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

namespace GKCommon.GEDCOM
{
    public class GEDCOMDateValue : GEDCOMCustomDate
    {
        private GEDCOMCustomDate fValue;

        public GEDCOMCustomDate Value
        {
            get { return fValue; }
        }

        protected override void CreateObj(GEDCOMTree owner, GEDCOMObject parent)
        {
            base.CreateObj(owner, parent);
            fValue = null;
        }

        protected override string GetStringValue()
        {
            return ((fValue == null) ? "" : fValue.StringValue);
        }

        public override DateTime GetDateTime()
        {
            DateTime result = ((fValue == null) ? new DateTime(0) : fValue.GetDateTime());
            return result;
        }

        public override void SetDateTime(DateTime value)
        {
            if (fValue != null)
            {
                fValue.SetDateTime(value);
            }
            else
            {
                fValue = new GEDCOMDateExact(Owner, this, "", "");
                fValue.Date = value;
            }
        }

        public override void Clear()
        {
            base.Clear();

            if (fValue != null) fValue.Clear();
        }

        public override bool IsEmpty()
        {
            return base.IsEmpty() && (fValue == null || fValue.IsEmpty());
        }

        public override string ParseString(string strValue)
        {
            try
            {
                if (fValue != null) {
                    fValue.Dispose();
                    fValue = null;
                }

                if (string.IsNullOrEmpty(strValue)) {
                    return "";
                }

                string su = strValue.Substring(0, 3).ToUpperInvariant();

                if (su == GEDCOMDateApproximatedArray[1] || su == GEDCOMDateApproximatedArray[2] || su == GEDCOMDateApproximatedArray[3])
                {
                    fValue = new GEDCOMDateApproximated(Owner, this, "", "");
                }
                else if (su == "INT")
                {
                    fValue = new GEDCOMDateInterpreted(Owner, this, "", "");
                }
                else if (su == GEDCOMDateRangeArray[0] || su == GEDCOMDateRangeArray[1] || su == GEDCOMDateRangeArray[2])
                {
                    fValue = new GEDCOMDateRange(Owner, this, "", "");
                }
                else if (strValue.StartsWith("FROM", StringComparison.InvariantCulture) || strValue.StartsWith("TO", StringComparison.InvariantCulture))
                {
                    fValue = new GEDCOMDatePeriod(Owner, this, "", "");
                }
                else
                {
                    fValue = new GEDCOMDate(Owner, this, "", "");
                }

                return fValue.ParseString(strValue);
            }
            catch (Exception ex)
            {
                Logger.LogWrite("GEDCOMDateValue.ParseString(\"" + strValue + "\"): " + ex.Message);
                return strValue;
            }
        }

        public override void ResetOwner(GEDCOMTree newOwner)
        {
            base.ResetOwner(newOwner);
            if (fValue != null) fValue.ResetOwner(newOwner);
        }

        public GEDCOMDateValue(GEDCOMTree owner, GEDCOMObject parent, string tagName, string tagValue) : base(owner, parent, tagName, tagValue)
        {
        }

        public new static GEDCOMTag Create(GEDCOMTree owner, GEDCOMObject parent, string tagName, string tagValue)
        {
            return new GEDCOMDateValue(owner, parent, tagName, tagValue);
        }

        #region Auxiliary

        public override float IsMatch(GEDCOMTag tag, MatchParams matchParams)
        {
            if (tag == null) return 0.0f;
            GEDCOMDateValue date = (GEDCOMDateValue)tag;

            if (IsEmpty() || date.IsEmpty()) return 0.0f;

            int absVal1 = GEDCOMUtils.GetRelativeYear(this);
            int absVal2 = GEDCOMUtils.GetRelativeYear(date);

            float match = 0.0f;
            float matches = 0.0f;

            if (absVal1 != 0 && absVal2 != 0)
            {
                matches += 1.0f;
                if (Math.Abs(absVal1 - absVal2) <= matchParams.YearsInaccuracy) match += 100.0f;
            }

            return (match / matches);
        }

        public override void GetDateParts(out int year, out ushort month, out ushort day, out bool yearBC)
        {
            if (fValue == null) {
                year = -1;
                month = 0;
                day = 0;
                yearBC = false;
            } else {
                fValue.GetDateParts(out year, out month, out day, out yearBC);
            }
        }

        public override UDN GetUDN()
        {
            return (fValue == null) ? UDN.CreateEmpty() : fValue.GetUDN();
        }

        #endregion
    }
}
