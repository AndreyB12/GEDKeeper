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

#if !__MonoCS__

using GKCommon;
using GKCommon.GEDCOM;
using GKCore.Interfaces;
using GKTests.ControlTesters;
using GKTests.Mocks;
using GKUI.Dialogs;
using GKUI.Sheets;
using NUnit.Extensions.Forms;
using NUnit.Framework;

namespace GKTests.UITests
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class GroupEditDlgTests : CustomWindowTest
    {
        private GEDCOMGroupRecord fGroupRecord;
        private IBaseWindow fBase;
        private GroupEditDlg fDialog;

        public override void Setup()
        {
            base.Setup();

            fBase = new BaseWindowMock();
            fGroupRecord = new GEDCOMGroupRecord(fBase.Context.Tree, fBase.Context.Tree, "", "");

            fDialog = new GroupEditDlg(fBase);
            fDialog.Group = fGroupRecord;
            fDialog.Show();
        }

        [Test]
        public void Test_Cancel()
        {
            ClickButton("btnCancel", fDialog);
        }

        [Test]
        public void Test_EnterDataAndApply()
        {
            Assert.AreEqual(fBase, fDialog.Base);
            Assert.AreEqual(fGroupRecord, fDialog.Group);

            var sheetTester = new GKSheetListTester("fMembersList", fDialog);
            EnumSet<SheetButton> buttons = sheetTester.Properties.Buttons;
            Assert.IsTrue(buttons.ContainsAll(SheetButton.lbAdd, SheetButton.lbDelete, SheetButton.lbJump));
            Assert.IsFalse(sheetTester.Properties.ReadOnly);

            var edName = new TextBoxTester("edName");
            edName.Enter("sample text");

            ClickButton("btnAccept", fDialog);

            Assert.AreEqual("sample text", fGroupRecord.GroupName);
        }
    }
}

#endif
