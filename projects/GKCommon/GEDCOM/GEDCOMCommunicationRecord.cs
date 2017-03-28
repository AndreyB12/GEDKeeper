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

namespace GKCommon.GEDCOM
{
    public sealed class GEDCOMCommunicationRecord : GEDCOMRecord
    {
        public static readonly string[] CommunicationTags = new string[] { "FROM", "TO" };

        public GEDCOMDateExact Date
        {
            get { return TagClass("DATE", GEDCOMDateExact.Create) as GEDCOMDateExact; }
        }

        public string CommName
        {
            get { return GetTagStringValue("NAME"); }
            set { SetTagStringValue("NAME", value); }
        }

        public GKCommunicationType CommunicationType
        {
            get { return GEDCOMUtils.GetCommunicationTypeVal(GetTagStringValue("TYPE")); }
            set { SetTagStringValue("TYPE", GEDCOMUtils.GetCommunicationTypeStr(value)); }
        }

        protected override void CreateObj(GEDCOMTree owner, GEDCOMObject parent)
        {
            base.CreateObj(owner, parent);
            SetRecordType(GEDCOMRecordType.rtCommunication);
            SetName("_COMM");
        }

        public override GEDCOMTag AddTag(string tagName, string tagValue, TagConstructor tagConstructor)
        {
            GEDCOMTag result;

            if (tagName == "NAME")
            {
                result = base.AddTag(tagName, tagValue, null);
            }
            else if (tagName == "DATE")
            {
                result = base.AddTag(tagName, tagValue, GEDCOMDateExact.Create);
            }
            else
            {
                result = base.AddTag(tagName, tagValue, tagConstructor);
            }

            return result;
        }

        public GEDCOMCommunicationRecord(GEDCOMTree owner, GEDCOMObject parent, string tagName, string tagValue) : base(owner, parent, tagName, tagValue)
        {
        }

        public new static GEDCOMTag Create(GEDCOMTree owner, GEDCOMObject parent, string tagName, string tagValue)
        {
            return new GEDCOMCommunicationRecord(owner, parent, tagName, tagValue);
        }
        
        #region Auxiliary

        public void GetCorresponder(out GKCommunicationDir commDir, out GEDCOMIndividualRecord corresponder)
        {
            commDir = GKCommunicationDir.cdFrom;
            corresponder = null;

            GEDCOMTag corrTag = FindTag("FROM", 0);
            if (corrTag == null) {
                corrTag = FindTag("TO", 0);
            }

            if (corrTag != null) {
                corresponder = (Owner.XRefIndex_Find(GEDCOMUtils.CleanXRef(corrTag.StringValue)) as GEDCOMIndividualRecord);

                if (corrTag.Name == "FROM") {
                    commDir = GKCommunicationDir.cdFrom;
                } else if (corrTag.Name == "TO") {
                    commDir = GKCommunicationDir.cdTo;
                }
            }
        }

        public void SetCorresponder(GKCommunicationDir commDir, GEDCOMIndividualRecord corresponder)
        {
            DeleteTag("FROM");
            DeleteTag("TO");

            if (corresponder != null) {
                AddTag(CommunicationTags[(int)commDir], GEDCOMUtils.EncloseXRef(corresponder.XRef), null);
            }
        }

        #endregion
    }
}
