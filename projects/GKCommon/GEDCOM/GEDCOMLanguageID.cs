/*
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
    public enum GEDCOMLanguageID
    {
        Unknown,

        Afrikaans,
        Albanian,
        AngloSaxon,
        Catalan,
        CatalanSpn,
        Czech,
        Danish,
        Dutch,
        English,
        Esperanto,
        Estonian,
        Faroese,
        Finnish,
        French,
        German,
        Hawaiian,
        Hungarian,
        Icelandic,
        Indonesian,
        Italian,
        Latvian,
        Lithuanian,
        Navaho,
        Norwegian,
        Polish,
        Portuguese,
        Romanian,
        SerboCroatian,
        Slovak,
        Slovene,
        Spanish,
        Swedish,
        Turkish,
        Wendic,

        Amharic,
        Arabic,
        Armenian,
        Assamese,
        Belorusian,
        Bengali,
        Braj,
        Bulgarian,
        Burmese,
        Cantonese,
        ChurchSlavic,
        Dogri,
        Georgian,
        Greek,
        Gujarati,
        Hebrew,
        Hindi,
        Japanese,
        Kannada,
        Khmer,
        Konkani,
        Korean,
        Lahnda,
        Lao,
        Macedonian,
        Maithili,
        Malayalam,
        Mandrin,
        Manipuri,
        Marathi,
        Mewari,
        Nepali,
        Oriya,
        Pahari,
        Palio,
        Panjabi,
        Persian,
        Prakrit,
        Pusto,
        Rajasthani,
        Russian,
        Sanskrit,
        Serb,
        Tagalog,
        Tamil,
        Telugu,
        Thai,
        Tibetan,
        Ukrainian,
        Urdu,
        Vietnamese,
        Yiddish
    }

    public sealed class GEDCOMLanguageEnumHelper : GEDCOMEnumHelper<GEDCOMLanguageID>
    {
        public GEDCOMLanguageEnumHelper() : base(GEDCOMConsts.LngEnumStr, GEDCOMLanguageID.Unknown, true)
        {
        }
    }
}
