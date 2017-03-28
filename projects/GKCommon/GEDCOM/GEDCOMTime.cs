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
    public sealed class GEDCOMTime : GEDCOMTag
    {
        private ushort fHour;
        private ushort fMinutes;
        private ushort fSeconds;
        private ushort fFraction;

        public ushort Fraction
        {
            get { return fFraction; }
            set { fFraction = value; }
        }

        public ushort Hour
        {
            get { return fHour; }
            set { fHour = value; }
        }

        public ushort Minutes
        {
            get { return fMinutes; }
            set { fMinutes = value; }
        }

        public ushort Seconds
        {
            get { return fSeconds; }
            set { fSeconds = value; }
        }

        public TimeSpan Value
        {
            get {
                return new TimeSpan(0, fHour, fMinutes, fSeconds, (int)(100u * fFraction));
            }
            set {
                fHour = (ushort)value.Hours;
                fMinutes = (ushort)value.Minutes;
                fSeconds = (ushort)value.Seconds;
                ushort mSec = (ushort)value.Milliseconds;
                fFraction = (ushort)Math.Truncate(mSec / 100.0);
            }
        }

        protected override void CreateObj(GEDCOMTree owner, GEDCOMObject parent)
        {
            base.CreateObj(owner, parent);
            SetName("TIME");
        }

        protected override string GetStringValue()
        {
            string result;
            if (fHour == 0 && fMinutes == 0 && fSeconds == 0)
            {
                result = "";
            }
            else
            {
                result = string.Format("{0:00}:{1:00}:{2:00}", new object[] { fHour, fMinutes, fSeconds });

                if (fFraction > 0)
                {
                    result = result + "." + fFraction.ToString();
                }
            }
            return result;
        }

        public override void Clear()
        {
            base.Clear();
            fHour = 0;
            fMinutes = 0;
            fSeconds = 0;
            fFraction = 0;
        }

        public override bool IsEmpty()
        {
            return base.IsEmpty() && fHour == 0 && fMinutes == 0 && fSeconds == 0;
        }

        public override string ParseString(string strValue)
        {
            fHour = 0;
            fMinutes = 0;
            fSeconds = 0;
            fFraction = 0;

            string result = strValue;
            if (!string.IsNullOrEmpty(result))
            {
                result = GEDCOMUtils.ExtractDelimiter(result, 0);

                int tmp;
                result = GEDCOMUtils.ExtractNumber(result, out tmp, false, 0);
                fHour = (ushort)tmp;
                if (result != "" && result[0] == ':')
                {
                    result = result.Remove(0, 1);
                }

                result = GEDCOMUtils.ExtractNumber(result, out tmp, false, 0);
                fMinutes = (ushort)tmp;
                if (result != "" && result[0] == ':')
                {
                    result = result.Remove(0, 1);

                    result = GEDCOMUtils.ExtractNumber(result, out tmp, false, 0);
                    fSeconds = (ushort)tmp;
                    if (result != "" && result[0] == '.')
                    {
                        result = result.Remove(0, 1);

                        result = GEDCOMUtils.ExtractNumber(result, out tmp, false, 0);
                        fFraction = (ushort)tmp;
                    }
                }
            }
            return result;
        }

        public GEDCOMTime(GEDCOMTree owner, GEDCOMObject parent, string tagName, string tagValue) : base(owner, parent, tagName, tagValue)
        {
        }

        public new static GEDCOMTag Create(GEDCOMTree owner, GEDCOMObject parent, string tagName, string tagValue)
        {
            return new GEDCOMTime(owner, parent, tagName, tagValue);
        }
    }
}
