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
using System.Windows.Forms;

using GKCommon;
using GKCommon.Controls;
using GKCommon.GEDCOM;
using GKCore;
using GKCore.Interfaces;
using GKCore.Lists;
using GKCore.Operations;
using GKCore.Types;
using GKUI.Sheets;

namespace GKUI.Dialogs
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ResearchEditDlg : EditorDialog
    {
        private readonly GKSheetList fTasksList;
        private readonly GKSheetList fCommunicationsList;
        private readonly GKSheetList fGroupsList;
        private readonly GKSheetList fNotesList;

        private GEDCOMResearchRecord fResearch;

        public GEDCOMResearchRecord Research
        {
            get { return fResearch; }
            set { SetResearch(value); }
        }

        private void SetResearch(GEDCOMResearchRecord value)
        {
            fResearch = value;
            try
            {
                if (fResearch == null)
                {
                    txtName.Text = "";
                    cmbPriority.SelectedIndex = -1;
                    cmbStatus.SelectedIndex = -1;
                    txtStartDate.Text = "";
                    txtStopDate.Text = "";
                    nudPercent.Text = @"0";
                }
                else
                {
                    txtName.Text = fResearch.ResearchName;
                    cmbPriority.SelectedIndex = (int)fResearch.Priority;
                    cmbStatus.SelectedIndex = (int)fResearch.Status;
                    txtStartDate.Text = GKUtils.GetDateFmtString(fResearch.StartDate, DateFormat.dfDD_MM_YYYY);
                    txtStopDate.Text = GKUtils.GetDateFmtString(fResearch.StopDate, DateFormat.dfDD_MM_YYYY);
                    nudPercent.Text = fResearch.Percent.ToString();
                }

                fNotesList.ListModel.DataOwner = fResearch;

                UpdateLists();
            }
            catch (Exception ex)
            {
                fBase.Host.LogWrite("ResearchEditDlg.SetResearch(): " + ex.Message);
            }
        }

        public ResearchEditDlg(IBaseWindow baseWin) : base(baseWin)
        {
            InitializeComponent();

            btnAccept.Image = GKResources.iBtnAccept;
            btnCancel.Image = GKResources.iBtnCancel;

            for (GKResearchPriority rp = GKResearchPriority.rpNone; rp <= GKResearchPriority.rpTop; rp++)
            {
                cmbPriority.Items.Add(LangMan.LS(GKData.PriorityNames[(int)rp]));
            }

            for (GKResearchStatus rs = GKResearchStatus.rsDefined; rs <= GKResearchStatus.rsWithdrawn; rs++)
            {
                cmbStatus.Items.Add(LangMan.LS(GKData.StatusNames[(int)rs]));
            }

            fTasksList = new GKSheetList(pageTasks);
            fTasksList.OnModify += ListTasksModify;
            fTasksList.Buttons = EnumSet<SheetButton>.Create(
                SheetButton.lbAdd, SheetButton.lbEdit, SheetButton.lbDelete, SheetButton.lbJump
               );
            fTasksList.AddColumn(LangMan.LS(LSID.LSID_Goal), 250, false);
            fTasksList.AddColumn(LangMan.LS(LSID.LSID_Priority), 90, false);
            fTasksList.AddColumn(LangMan.LS(LSID.LSID_StartDate), 90, false);
            fTasksList.AddColumn(LangMan.LS(LSID.LSID_StopDate), 90, false);
            fTasksList.SetControlName("fTasksList"); // for purpose of tests

            fCommunicationsList = new GKSheetList(pageCommunications);
            fCommunicationsList.OnModify += ListCommunicationsModify;
            fCommunicationsList.Buttons = EnumSet<SheetButton>.Create(
                SheetButton.lbAdd, SheetButton.lbEdit, SheetButton.lbDelete, SheetButton.lbJump
               );
            fCommunicationsList.AddColumn(LangMan.LS(LSID.LSID_Theme), 150, false);
            fCommunicationsList.AddColumn(LangMan.LS(LSID.LSID_Corresponder), 150, false);
            fCommunicationsList.AddColumn(LangMan.LS(LSID.LSID_Type), 90, false);
            fCommunicationsList.AddColumn(LangMan.LS(LSID.LSID_Date), 90, false);
            fCommunicationsList.SetControlName("fCommunicationsList"); // for purpose of tests

            fGroupsList = new GKSheetList(pageGroups);
            fGroupsList.OnModify += ListGroupsModify;
            fGroupsList.Buttons = EnumSet<SheetButton>.Create(
                SheetButton.lbAdd, SheetButton.lbEdit, SheetButton.lbDelete, SheetButton.lbJump
               );
            fGroupsList.AddColumn(LangMan.LS(LSID.LSID_Group), 350, false);
            fGroupsList.SetControlName("fGroupsList"); // for purpose of tests

            fNotesList = new GKSheetList(pageNotes, new GKNotesListModel(fBase, fLocalUndoman));

            SetLang();
        }

        public void SetLang()
        {
            btnAccept.Text = LangMan.LS(LSID.LSID_DlgAccept);
            btnCancel.Text = LangMan.LS(LSID.LSID_DlgCancel);
            Text = LangMan.LS(LSID.LSID_WinResearchEdit);
            pageTasks.Text = LangMan.LS(LSID.LSID_RPTasks);
            pageCommunications.Text = LangMan.LS(LSID.LSID_RPCommunications);
            pageGroups.Text = LangMan.LS(LSID.LSID_RPGroups);
            pageNotes.Text = LangMan.LS(LSID.LSID_RPNotes);
            lblName.Text = LangMan.LS(LSID.LSID_Title);
            lblPriority.Text = LangMan.LS(LSID.LSID_Priority);
            lblStatus.Text = LangMan.LS(LSID.LSID_Status);
            lblPercent.Text = LangMan.LS(LSID.LSID_Percent);
            lblStartDate.Text = LangMan.LS(LSID.LSID_StartDate);
            lblStopDate.Text = LangMan.LS(LSID.LSID_StopDate);
        }

        private void ListTasksModify(object sender, ModifyEventArgs eArgs)
        {
            bool res = false;

            GEDCOMTaskRecord task = eArgs.ItemData as GEDCOMTaskRecord;

            switch (eArgs.Action)
            {
                case RecordAction.raAdd:
                    task = fBase.SelectRecord(GEDCOMRecordType.rtTask, null) as GEDCOMTaskRecord;
                    //res = fResearch.AddTask(task);
                    res = fLocalUndoman.DoOrdinaryOperation(OperationType.otResearchTaskAdd, fResearch, task);
                    break;

                case RecordAction.raEdit:
                    res = (task != null && fBase.ModifyTask(ref task));
                    break;

                case RecordAction.raDelete:
                    if (task != null && GKUtils.ShowQuestion(LangMan.LS(LSID.LSID_DetachTaskQuery)) != DialogResult.No)
                    {
                        //fResearch.RemoveTask(task);
                        //res = true;
                        res = fLocalUndoman.DoOrdinaryOperation(OperationType.otResearchTaskRemove, fResearch, task);
                    }
                    break;

                case RecordAction.raJump:
                    if (task != null)
                    {
                        AcceptChanges();
                        fBase.SelectRecordByXRef(task.XRef);
                        Close();
                    }
                    break;
            }

            if (res) UpdateLists();
        }

        private void ListCommunicationsModify(object sender, ModifyEventArgs eArgs)
        {
            bool res = false;

            GEDCOMCommunicationRecord comm = eArgs.ItemData as GEDCOMCommunicationRecord;

            switch (eArgs.Action)
            {
                case RecordAction.raAdd:
                    comm = fBase.SelectRecord(GEDCOMRecordType.rtCommunication, null) as GEDCOMCommunicationRecord;
                    //res = fResearch.AddCommunication(comm);
                    res = fLocalUndoman.DoOrdinaryOperation(OperationType.otResearchCommunicationAdd, fResearch, comm);
                    break;

                case RecordAction.raEdit:
                    res = (comm != null && fBase.ModifyCommunication(ref comm));
                    break;

                case RecordAction.raDelete:
                    if (comm != null && GKUtils.ShowQuestion(LangMan.LS(LSID.LSID_DetachCommunicationQuery)) != DialogResult.No)
                    {
                        //fResearch.RemoveCommunication(comm);
                        //res = true;
                        res = fLocalUndoman.DoOrdinaryOperation(OperationType.otResearchCommunicationRemove, fResearch, comm);
                    }
                    break;

                case RecordAction.raJump:
                    if (comm != null)
                    {
                        AcceptChanges();
                        fBase.SelectRecordByXRef(comm.XRef);
                        Close();
                    }
                    break;
            }

            if (res) UpdateLists();
        }

        private void ListGroupsModify(object sender, ModifyEventArgs eArgs)
        {
            bool res = false;

            GEDCOMGroupRecord group = eArgs.ItemData as GEDCOMGroupRecord;

            switch (eArgs.Action)
            {
                case RecordAction.raAdd:
                    group = fBase.SelectRecord(GEDCOMRecordType.rtGroup, null) as GEDCOMGroupRecord;
                    //res = fResearch.AddGroup(group);
                    res = fLocalUndoman.DoOrdinaryOperation(OperationType.otResearchGroupAdd, fResearch, group);
                    break;

                case RecordAction.raDelete:
                    if (group != null && GKUtils.ShowQuestion(LangMan.LS(LSID.LSID_DetachGroupQuery)) != DialogResult.No)
                    {
                        //fResearch.RemoveGroup(group);
                        //res = true;
                        res = fLocalUndoman.DoOrdinaryOperation(OperationType.otResearchGroupRemove, fResearch, group);
                    }
                    break;

                case RecordAction.raJump:
                    if (group != null)
                    {
                        AcceptChanges();
                        fBase.SelectRecordByXRef(group.XRef);
                        Close();
                    }
                    break;
            }

            if (res) UpdateLists();
        }

        private void UpdateLists()
        {
            if (fResearch == null) {
                fTasksList.ClearItems();
                fCommunicationsList.ClearItems();
                fGroupsList.ClearItems();

                return;
            }

            fNotesList.UpdateSheet();

            fTasksList.BeginUpdate();
            fTasksList.ClearItems();
            foreach (GEDCOMPointer taskPtr in fResearch.Tasks)
            {
                GEDCOMTaskRecord task = taskPtr.Value as GEDCOMTaskRecord;
                if (task == null) continue;

                GKListItem item = fTasksList.AddItem(GKUtils.GetTaskGoalStr(task), task);
                item.AddSubItem(LangMan.LS(GKData.PriorityNames[(int)task.Priority]));
                item.AddSubItem(new GEDCOMDateItem(task.StartDate));
                item.AddSubItem(new GEDCOMDateItem(task.StopDate));
            }
            fTasksList.EndUpdate();

            fCommunicationsList.BeginUpdate();
            fCommunicationsList.ClearItems();
            foreach (GEDCOMPointer commPtr in fResearch.Communications)
            {
                GEDCOMCommunicationRecord corr = commPtr.Value as GEDCOMCommunicationRecord;
                if (corr == null) continue;

                GKListItem item = fCommunicationsList.AddItem(corr.CommName, corr);
                item.AddSubItem(GKUtils.GetCorresponderStr(fBase.Tree, corr, false));
                item.AddSubItem(LangMan.LS(GKData.CommunicationNames[(int)corr.CommunicationType]));
                item.AddSubItem(new GEDCOMDateItem(corr.Date));
            }
            fCommunicationsList.EndUpdate();

            fGroupsList.BeginUpdate();
            fGroupsList.ClearItems();
            foreach (GEDCOMPointer groupPtr in fResearch.Groups)
            {
                GEDCOMGroupRecord grp = groupPtr.Value as GEDCOMGroupRecord;
                if (grp == null) continue;

                fGroupsList.AddItem(grp.GroupName, grp);
            }
            fGroupsList.EndUpdate();
        }

        private void AcceptChanges()
        {
            fResearch.ResearchName = txtName.Text;
            fResearch.Priority = (GKResearchPriority)cmbPriority.SelectedIndex;
            fResearch.Status = (GKResearchStatus)cmbStatus.SelectedIndex;
            fResearch.StartDate.ParseString(GEDCOMUtils.StrToGEDCOMDate(txtStartDate.Text, true));
            fResearch.StopDate.ParseString(GEDCOMUtils.StrToGEDCOMDate(txtStopDate.Text, true));
            fResearch.Percent = int.Parse(nudPercent.Text);

            CommitChanges();

            fBase.ChangeRecord(fResearch);
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
                fBase.Host.LogWrite("ResearchEditDlg.btnAccept_Click(): " + ex.Message);
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
                fBase.Host.LogWrite("ResearchEditDlg.btnCancel_Click(): " + ex.Message);
            }
        }
    }
}
