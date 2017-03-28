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

using System.IO;

namespace GKCommon.GEDCOM
{
    public abstract class GEDCOMCustomRecord : GEDCOMTag
    {
        private string fXRef;

        public string XRef
        {
            get { return fXRef; }
            set {
                string oldXRef = fXRef;
                fXRef = value;
                if (Owner != null) {
                    Owner.SetXRef(oldXRef, this);
                }
            }
        }

        protected override void SaveValueToStream(StreamWriter stream)
        {
            string str = Level.ToString();

            if (!string.IsNullOrEmpty(fXRef))
            {
                str = str + " " + "@" + fXRef + "@";
            }
            str = str + " " + Name;

            if (StringValue != "")
            {
                str = str + " " + StringValue;
            }

            stream.Write(str + GEDCOM_NEWLINE);
        }

        protected GEDCOMCustomRecord(GEDCOMTree owner, GEDCOMObject parent, string tagName, string tagValue) : base(owner, parent, tagName, tagValue)
        {
        }
    }
}
