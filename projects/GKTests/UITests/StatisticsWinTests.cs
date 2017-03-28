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

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using GKCommon.GEDCOM;
using GKCore.Interfaces;
using GKTests.Mocks;
using GKUI;
using NUnit.Extensions.Forms;
using NUnit.Framework;

namespace GKTests.UITests
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class StatisticsWinTests : CustomWindowTest
    {
        private IBaseWindow fBase;
        private StatisticsWin fDialog;

        public override void Setup()
        {
            base.Setup();

            fBase = new BaseWindowMock();

            fDialog = new StatisticsWin(fBase, new List<GEDCOMRecord>());
            fDialog.Show();
        }

        [Test]
        public void Test_Common()
        {
            var cbType = new ToolStripComboBoxTester("cbType", fDialog);

            for (int i = 0; i < cbType.Properties.Items.Count; i++) {
                cbType.Select(i);

                if (i == 0) {
                    //ModalFormHandler = GenerateExcel_Handler;
                    //ClickToolStripButton("tbExcelExport", fDialog);
                }
            }
        }

        private void GenerateExcel_Handler(string name, IntPtr hWnd, Form form)
        {
            PrepareFileSave("test.xls", hWnd);
        }
    }
}

#endif
