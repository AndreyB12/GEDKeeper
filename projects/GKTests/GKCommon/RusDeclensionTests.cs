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
using Externals.Linguistics;
using NUnit.Framework;

namespace GKTests.GKCommon
{
    [TestFixture]
    public class RusDeclensionTests
    {
        [Test]
        public void Test_Common()
        {
            Assert.AreEqual("Иванова Ивана Ивановича", RusDeclension.GetDeclension("Иванов Иван Иванович", DeclensionCase.Genitive));

            Assert.AreEqual("Иванова-Петрова Ивана Ивановича", RusDeclension.GetDeclension("Иванов-Петров Иван Иванович", DeclensionCase.Genitive));

            //Assert.AreEqual("атому", RusDeclension.GetDeclension("атом", DeclensionCase.Dative));
            //Assert.AreEqual("лугу", RusDeclension.GetDeclension("луг", DeclensionCase.Dative));
        }
    }
}
