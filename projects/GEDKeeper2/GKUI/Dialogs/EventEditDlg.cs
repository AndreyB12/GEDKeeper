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
using System.Drawing;
using System.Windows.Forms;

using GKCommon;
using GKCommon.GEDCOM;
using GKCore;
using GKCore.Interfaces;
using GKCore.Types;
using GKUI.Controls;
using GKUI.Sheets;

namespace GKUI.Dialogs
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class EventEditDlg : EditorDialog
    {
        private readonly GKSheetList fNotesList;
        private readonly GKSheetList fMediaList;
        private readonly GKSheetList fSourcesList;

        private GEDCOMCustomEvent fEvent;
        private GEDCOMLocationRecord fLocation;

        public GEDCOMCustomEvent Event
        {
            get { return fEvent; }
            set { SetEvent(value); }
        }

        public EventEditDlg(IBaseWindow baseWin) : base(baseWin)
        {
            InitializeComponent();

            btnAccept.Image = GKResources.iBtnAccept;
            btnCancel.Image = GKResources.iBtnCancel;
            btnPlaceAdd.Image = GKResources.iRecNew;
            btnPlaceDelete.Image = GKResources.iRecDelete;

            int num = GKData.DateKinds.Length;
            for (int i = 0; i < num; i++)
            {
                cmbEventDateType.Items.Add(LangMan.LS(GKData.DateKinds[i].Name));
            }

            for (GEDCOMCalendar gc = GEDCOMCalendar.dcGregorian; gc <= GEDCOMCalendar.dcLast; gc++)
            {
                GKData.CalendarStruct cdr = GKData.DateCalendars[(int)gc];
                if (!cdr.HasSupport) continue;

                cmbDate1Calendar.Items.Add(new GKComboItem(LangMan.LS(cdr.Name), gc));
                cmbDate2Calendar.Items.Add(new GKComboItem(LangMan.LS(cdr.Name), gc));
            }

            cmbDate1Calendar.SelectedIndex = 0;
            cmbDate2Calendar.SelectedIndex = 0;

            fLocation = null;

            fNotesList = new GKSheetList(pageNotes, new GKNotesListModel(fBase, fLocalUndoman));
            fMediaList = new GKSheetList(pageMultimedia, new GKMediaListModel(fBase, fLocalUndoman));
            fSourcesList = new GKSheetList(pageSources, new GKSourcesListModel(fBase, fLocalUndoman));

            // SetLang()
            Text = LangMan.LS(LSID.LSID_Event);
            btnAccept.Text = LangMan.LS(LSID.LSID_DlgAccept);
            btnCancel.Text = LangMan.LS(LSID.LSID_DlgCancel);
            btnAddress.Text = LangMan.LS(LSID.LSID_Address) + @"...";
            pageCommon.Text = LangMan.LS(LSID.LSID_Common);
            pageNotes.Text = LangMan.LS(LSID.LSID_RPNotes);
            pageMultimedia.Text = LangMan.LS(LSID.LSID_RPMultimedia);
            pageSources.Text = LangMan.LS(LSID.LSID_RPSources);
            lblEvent.Text = LangMan.LS(LSID.LSID_Event);
            lblAttrValue.Text = LangMan.LS(LSID.LSID_Value);
            lblPlace.Text = LangMan.LS(LSID.LSID_Place);
            lblDate.Text = LangMan.LS(LSID.LSID_Date);
            lblCause.Text = LangMan.LS(LSID.LSID_Cause);
            lblOrg.Text = LangMan.LS(LSID.LSID_Agency);

            toolTip1.SetToolTip(btnPlaceAdd, LangMan.LS(LSID.LSID_PlaceAddTip));
            toolTip1.SetToolTip(btnPlaceDelete, LangMan.LS(LSID.LSID_PlaceDeleteTip));
        }

        private static GEDCOMCalendar GetComboCalendar(ComboBox comboBox)
        {
            GEDCOMCalendar result = (GEDCOMCalendar)(((GKComboItem)comboBox.SelectedItem).Tag);
            return result;
        }

        private static void SetComboCalendar(ComboBox comboBox, GEDCOMCalendar calendar)
        {
            foreach (object item in comboBox.Items) {
                GKComboItem comboItem = (GKComboItem)item;

                if ((GEDCOMCalendar)comboItem.Tag == calendar) {
                    comboBox.SelectedItem = item;
                    return;
                }
            }

            comboBox.SelectedIndex = 0;
        }

        private string AssembleDate()
        {
            string result = "";

            GEDCOMCalendar cal1 = GetComboCalendar(cmbDate1Calendar);
            GEDCOMCalendar cal2 = GetComboCalendar(cmbDate2Calendar);

            string gcd1 = GEDCOMUtils.StrToGEDCOMDate(txtEventDate1.Text, true);
            string gcd2 = GEDCOMUtils.StrToGEDCOMDate(txtEventDate2.Text, true);

            if (cal1 != GEDCOMCalendar.dcGregorian) {
                gcd1 = GEDCOMCustomDate.GEDCOMDateEscapeArray[(int)cal1] + " " + gcd1;
            }

            if (cal2 != GEDCOMCalendar.dcGregorian) {
                gcd2 = GEDCOMCustomDate.GEDCOMDateEscapeArray[(int)cal2] + " " + gcd2;
            }

            if (btnBC1.Checked) {
                gcd1 = gcd1 + GEDCOMObject.GEDCOM_YEAR_BC;
            }

            if (btnBC2.Checked) {
                gcd2 = gcd2 + GEDCOMObject.GEDCOM_YEAR_BC;
            }

            switch (cmbEventDateType.SelectedIndex) {
                case 0:
                    result = gcd1;
                    break;

                case 1:
                    result = "BEF " + gcd2;
                    break;

                case 2:
                    result = "AFT " + gcd1;
                    break;

                case 3:
                    result = "BET " + gcd1 + " AND " + gcd2;
                    break;

                case 4:
                    result = "FROM " + gcd1;
                    break;

                case 5:
                    result = "TO " + gcd2;
                    break;

                case 6:
                    result = "FROM " + gcd1 + " TO " + gcd2;
                    break;

                case 7:
                    result = "ABT " + gcd1;
                    break;

                case 8:
                    result = "CAL " + gcd1;
                    break;

                case 9:
                    result = "EST " + gcd1;
                    break;
            }

            return result;
        }

        private void AcceptChanges()
        {
            fEvent.Place.StringValue = txtEventPlace.Text;
            fEvent.Place.Location.Value = fLocation;
            fEvent.Classification = txtEventName.Text;
            fEvent.Cause = txtEventCause.Text;
            fEvent.Agency = txtEventOrg.Text;

            string dt = AssembleDate();
            fEvent.Date.ParseString(dt);

            if (fEvent is GEDCOMFamilyEvent)
            {
                fEvent.SetName(GKData.FamilyEvents[cmbEventType.SelectedIndex].Sign);
            }
            else
            {
                int id = cmbEventType.SelectedIndex;
                fEvent.SetName(GKData.PersonEvents[id].Sign);
                if (GKData.PersonEvents[id].Kind == PersonEventKind.ekFact)
                {
                    fEvent.StringValue = txtAttribute.Text;
                }
                else
                {
                    fEvent.StringValue = "";
                }
            }

            if (fEvent is GEDCOMIndividualEvent)
            {
                int id = cmbEventType.SelectedIndex;
                if (GKData.PersonEvents[id].Kind == PersonEventKind.ekFact)
                {
                    GEDCOMIndividualAttribute attr = new GEDCOMIndividualAttribute(fEvent.Owner, fEvent.Parent, "", "");
                    attr.Assign(fEvent);
                    fEvent = attr;
                }
            }

            CommitChanges();
        }

        private void ControlsRefresh()
        {
            if (fLocation != null) {
                txtEventPlace.Text = fLocation.LocationName;
                txtEventPlace.ReadOnly = true;
                txtEventPlace.BackColor = SystemColors.Control;
                btnPlaceAdd.Enabled = false;
                btnPlaceDelete.Enabled = true;
            } else {
                txtEventPlace.Text = fEvent.Place.StringValue;
                txtEventPlace.ReadOnly = false;
                txtEventPlace.BackColor = SystemColors.Window;
                btnPlaceAdd.Enabled = true;
                btnPlaceDelete.Enabled = false;
            }

            fNotesList.UpdateSheet();
            fMediaList.UpdateSheet();
            fSourcesList.UpdateSheet();
        }

        private void SetEvent(GEDCOMCustomEvent value)
        {
            fEvent = value;

            if (fEvent is GEDCOMFamilyEvent)
            {
                int num = GKData.FamilyEvents.Length;
                for (int i = 0; i < num; i++)
                {
                    cmbEventType.Items.Add(LangMan.LS(GKData.FamilyEvents[i].Name));
                }

                int idx = GKUtils.GetFamilyEventIndex(fEvent.Name);
                if (idx < 0) idx = 0;
                cmbEventType.SelectedIndex = idx;
            }
            else
            {
                int num = GKData.PersonEvents.Length;
                for (int i = 0; i < num; i++)
                {
                    cmbEventType.Items.Add(LangMan.LS(GKData.PersonEvents[i].Name));
                }

                int idx = GKUtils.GetPersonEventIndex(fEvent.Name);
                if (idx < 0) idx = 0;
                cmbEventType.SelectedIndex = idx;

                if (idx >= 0 && GKData.PersonEvents[idx].Kind == PersonEventKind.ekFact)
                {
                    txtAttribute.Text = fEvent.StringValue;
                }
            }

            EditEventType_SelectedIndexChanged(null, null);

            GEDCOMCustomDate date = fEvent.Date.Value;
            if (date is GEDCOMDateApproximated)
            {
                GEDCOMApproximated approximated = (date as GEDCOMDateApproximated).Approximated;

                switch (approximated) {
                    case GEDCOMApproximated.daExact:
                        cmbEventDateType.SelectedIndex = 0;
                        break;
                    case GEDCOMApproximated.daAbout:
                        cmbEventDateType.SelectedIndex = 7;
                        break;
                    case GEDCOMApproximated.daCalculated:
                        cmbEventDateType.SelectedIndex = 8;
                        break;
                    case GEDCOMApproximated.daEstimated:
                        cmbEventDateType.SelectedIndex = 9;
                        break;
                }

                txtEventDate1.Text = GKUtils.GetDateFmtString(date as GEDCOMDate, DateFormat.dfDD_MM_YYYY);
                SetComboCalendar(cmbDate1Calendar, (date as GEDCOMDate).DateCalendar);
                btnBC1.Checked = (date as GEDCOMDate).YearBC;
            }
            else
            {
                if (date is GEDCOMDateRange)
                {
                    GEDCOMDateRange dtRange = date as GEDCOMDateRange;
                    if (dtRange.After.StringValue == "" && dtRange.Before.StringValue != "")
                    {
                        cmbEventDateType.SelectedIndex = 1;
                    }
                    else
                    {
                        if (dtRange.After.StringValue != "" && dtRange.Before.StringValue == "")
                        {
                            cmbEventDateType.SelectedIndex = 2;
                        }
                        else
                        {
                            if (dtRange.After.StringValue != "" && dtRange.Before.StringValue != "")
                            {
                                cmbEventDateType.SelectedIndex = 3;
                            }
                        }
                    }

                    txtEventDate1.Text = GKUtils.GetDateFmtString(dtRange.After, DateFormat.dfDD_MM_YYYY);
                    txtEventDate2.Text = GKUtils.GetDateFmtString(dtRange.Before, DateFormat.dfDD_MM_YYYY);
                    SetComboCalendar(cmbDate1Calendar, dtRange.After.DateCalendar);
                    SetComboCalendar(cmbDate2Calendar, dtRange.Before.DateCalendar);
                    btnBC1.Checked = dtRange.After.YearBC;
                    btnBC2.Checked = dtRange.Before.YearBC;
                }
                else
                {
                    if (date is GEDCOMDatePeriod)
                    {
                        GEDCOMDatePeriod dtPeriod = date as GEDCOMDatePeriod;
                        if (dtPeriod.DateFrom.StringValue != "" && dtPeriod.DateTo.StringValue == "")
                        {
                            cmbEventDateType.SelectedIndex = 4;
                        }
                        else
                        {
                            if (dtPeriod.DateFrom.StringValue == "" && dtPeriod.DateTo.StringValue != "")
                            {
                                cmbEventDateType.SelectedIndex = 5;
                            }
                            else
                            {
                                if (dtPeriod.DateFrom.StringValue != "" && dtPeriod.DateTo.StringValue != "")
                                {
                                    cmbEventDateType.SelectedIndex = 6;
                                }
                            }
                        }

                        txtEventDate1.Text = GKUtils.GetDateFmtString(dtPeriod.DateFrom, DateFormat.dfDD_MM_YYYY);
                        txtEventDate2.Text = GKUtils.GetDateFmtString(dtPeriod.DateTo, DateFormat.dfDD_MM_YYYY);
                        SetComboCalendar(cmbDate1Calendar, dtPeriod.DateFrom.DateCalendar);
                        SetComboCalendar(cmbDate2Calendar, dtPeriod.DateTo.DateCalendar);
                        btnBC1.Checked = dtPeriod.DateFrom.YearBC;
                        btnBC2.Checked = dtPeriod.DateTo.YearBC;
                    }
                    else
                    {
                        if (date is GEDCOMDate)
                        {
                            cmbEventDateType.SelectedIndex = 0;
                            txtEventDate1.Text = GKUtils.GetDateFmtString(date as GEDCOMDate, DateFormat.dfDD_MM_YYYY);
                            SetComboCalendar(cmbDate1Calendar, (date as GEDCOMDate).DateCalendar);
                            btnBC1.Checked = (date as GEDCOMDate).YearBC;
                        }
                        else
                        {
                            cmbEventDateType.SelectedIndex = 0;
                            txtEventDate1.Text = "";
                            cmbDate1Calendar.SelectedIndex = 0;
                            btnBC1.Checked = false;
                        }
                    }
                }
            }

            EditEventDateType_SelectedIndexChanged(null, null);
            txtEventName.Text = fEvent.Classification;
            txtEventCause.Text = fEvent.Cause;
            txtEventOrg.Text = fEvent.Agency;
            fLocation = (fEvent.Place.Location.Value as GEDCOMLocationRecord);

            fNotesList.ListModel.DataOwner = fEvent;
            fMediaList.ListModel.DataOwner = fEvent;
            fSourcesList.ListModel.DataOwner = fEvent;

            ControlsRefresh();

            ActiveControl = cmbEventType;
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            try
            {
                AcceptChanges();
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                fBase.Host.LogWrite("EventEditDlg.btnAccept_Click(): " + ex.Message);
                DialogResult = DialogResult.None;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                RollbackChanges();
            }
            catch (Exception ex)
            {
                fBase.Host.LogWrite("EventEditDlg.btnCancel_Click(): " + ex.Message);
            }
        }

        private void btnAddress_Click(object sender, EventArgs e)
        {
            fBase.ModifyAddress(fEvent.Address);
        }

        private void EditEventPlace_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down && e.Control)
            {
                txtEventPlace.Text = txtEventPlace.Text.ToLower();
            }
        }

        private void btnPlaceAdd_Click(object sender, EventArgs e)
        {
            fLocation = (fBase.SelectRecord(GEDCOMRecordType.rtLocation, null) as GEDCOMLocationRecord);
            ControlsRefresh();
        }

        private void btnPlaceDelete_Click(object sender, EventArgs e)
        {
            fLocation = null;
            ControlsRefresh();
        }

        private void EditEventDate1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(typeof(string)) ? DragDropEffects.Move : DragDropEffects.None;
        }

        private void EditEventDate1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(typeof(string)))
                {
                    string txt = e.Data.GetData(typeof(string)) as string;
                    string[] dt = ((MaskedTextBox)sender).Text.Split('.');
                    ((MaskedTextBox)sender).Text = dt[0] + '.' + dt[1] + '.' + txt;
                }
            }
            catch (Exception ex)
            {
                fBase.Host.LogWrite("EventEditDlg.DragDrop(): " + ex.Message);
            }
        }

        private void EditEventType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fEvent is GEDCOMFamilyEvent)
            {
                txtAttribute.Enabled = false;
                txtAttribute.BackColor = SystemColors.Control;
            }
            else
            {
                int idx = cmbEventType.SelectedIndex;
                if (idx >= 0) {
                    if (GKData.PersonEvents[idx].Kind == PersonEventKind.ekEvent)
                    {
                        txtAttribute.Enabled = false;
                        txtAttribute.BackColor = SystemColors.Control;
                        txtAttribute.Text = "";
                    }
                    else
                    {
                        txtAttribute.Enabled = true;
                        txtAttribute.BackColor = SystemColors.Window;
                    }
                }
            }

            string evName;
            int id = cmbEventType.SelectedIndex;
            if (fEvent is GEDCOMFamilyEvent) {
                evName = GKData.FamilyEvents[id].Sign;
            } else {
                evName = GKData.PersonEvents[id].Sign;
            }

            // TODO: It is necessary to provide the registrable list of values for different tag types.
            string[] vals;
            bool canbeSorted, userInput;

            if (evName == "_BGRO") {
                vals = GKData.BloodGroups.Split('|');
                canbeSorted = false;
                userInput = false;
            } else {
                vals = fBase.Context.ValuesCollection.GetValues(evName);
                canbeSorted = true;
                userInput = true;
            }

            if (vals != null) {
                string tmp = txtAttribute.Text;
                txtAttribute.Sorted = false;

                txtAttribute.Items.Clear();
                txtAttribute.Items.AddRange(vals);

                txtAttribute.Sorted = canbeSorted;
                txtAttribute.Text = tmp;

                txtAttribute.DropDownStyle = (userInput) ? ComboBoxStyle.DropDown : ComboBoxStyle.DropDownList;
            }
        }

        public void SetControlEnabled(Control ctl, bool enabled)
        {
            if (ctl == null) return;

            ctl.Enabled = enabled;
            ctl.BackColor = enabled ? SystemColors.Window : SystemColors.Control;
        }

        private void EditEventDateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = cmbEventDateType.SelectedIndex;
            if (idx < 0 || idx >= GKData.DateKinds.Length) return;

            byte dates = GKData.DateKinds[idx].Dates;
            txtEventDate1.Enabled = SysUtils.IsSetBit(dates, 0);
            txtEventDate2.Enabled = SysUtils.IsSetBit(dates, 1);

            cmbDate1Calendar.Enabled = txtEventDate1.Enabled;
            cmbDate2Calendar.Enabled = txtEventDate2.Enabled;

            btnBC1.Enabled = txtEventDate1.Enabled;
            btnBC2.Enabled = txtEventDate2.Enabled;
        }
    }
}
