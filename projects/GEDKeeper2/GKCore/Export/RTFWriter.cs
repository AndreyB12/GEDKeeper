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

using Elistia.DotNetRtfWriter;

namespace GKCore.Export
{
    /// <summary>
    /// 
    /// </summary>
    public class RTFWriter : CustomWriter
    {
        private class FontStruct
        {
            public FontDescriptor FD;
            public float Size;
            public System.Drawing.Color OriginalColor;
            public ColorDescriptor Color;
            public bool Bold;
            public bool Underline;
        }

        private readonly Align[] iAlignments = new Align[] { Align.Left, Align.Center, Align.Right, Align.FullyJustify };

        private readonly RtfDocument fDocument;
        private RtfParagraph fParagraph;

        public RTFWriter()
        {
            PaperOrientation po = (fAlbumPage) ? PaperOrientation.Landscape : PaperOrientation.Portrait;
            fDocument = new RtfDocument(PaperSize.A4, po, Lcid.English);
        }

        public override void beginWrite()
        {
        }

        public override void endWrite()
        {
            fDocument.save(fFileName);
        }

        private static RtfCharFormat addParagraphChunk(RtfParagraph par, string text, object font)
        {
            FontStruct fntStr = (FontStruct)font;

            par.DefaultCharFormat.Font = fntStr.FD;

            int beg = par.Text.Length;
            par.Text.Append(text);
            int end = par.Text.Length - 1;

            RtfCharFormat fmt = par.addCharFormat(beg, end);
            fmt.Font = fntStr.FD;
            fmt.FgColor = fntStr.Color;
            fmt.FontSize = fntStr.Size;
            if (fntStr.Bold) fmt.FontStyle.addStyle(FontStyleFlag.Bold);
            if (fntStr.Underline) fmt.FontStyle.addStyle(FontStyleFlag.Underline);

            return fmt;
        }

        public override void addParagraph(string text, object font, TextAlignment alignment)
        {
            RtfParagraph par = fDocument.addParagraph();
            par.Alignment = iAlignments[(int)alignment];
            addParagraphChunk(par, text, font);
        }

        public override void addParagraph(string text, object font)
        {
            RtfParagraph par = fDocument.addParagraph();
            addParagraphChunk(par, text, font);
        }

        public override void addParagraphAnchor(string text, object font, string anchor)
        {
            RtfParagraph par = fDocument.addParagraph();
            RtfCharFormat fmt = addParagraphChunk(par, text, font);
            fmt.Bookmark = anchor;
        }

        public override void addParagraphLink(string text, object font, string link, object linkFont)
        {
            RtfParagraph par = fDocument.addParagraph();
            RtfCharFormat fmt = addParagraphChunk(par, text, font);
            fmt.LocalHyperlink = link;
        }

        public override object CreateFont(string name, float size, bool bold, bool underline, System.Drawing.Color color)
        {
            if (string.IsNullOrEmpty(name)) name = "Times New Roman";

            FontStruct fntStr = new FontStruct();
            fntStr.FD = fDocument.createFont(name);
            fntStr.OriginalColor = color;
            fntStr.Color = fDocument.createColor(new RtfColor(color));
            fntStr.Size = size;
            fntStr.Bold = bold;
            fntStr.Underline = underline;

            return fntStr;
        }

        public override void beginList()
        {
        }

        public override void endList()
        {
        }

        public override void addListItem(string text, object font)
        {
            RtfParagraph par = fDocument.addParagraph();

            FontStruct fntStr = (FontStruct)font;
            FontStruct symFont = (FontStruct)CreateFont("Symbol", fntStr.Size, fntStr.Bold, fntStr.Underline, fntStr.OriginalColor);

            addParagraphChunk(par, "\t· ", symFont);
            addParagraphChunk(par, text, font);
        }

        public override void addListItemLink(string text, object font, string link, object linkFont)
        {
            RtfParagraph par = fDocument.addParagraph();

            FontStruct fntStr = (FontStruct)font;
            FontStruct symFont = (FontStruct)CreateFont("Symbol", fntStr.Size, fntStr.Bold, fntStr.Underline, fntStr.OriginalColor);

            addParagraphChunk(par, "\t· ", symFont);
            addParagraphChunk(par, text, font);

            if (!string.IsNullOrEmpty(link)) {
                RtfCharFormat fmt = addParagraphChunk(par, link, linkFont);
                fmt.LocalHyperlink = link;
            }
        }

        public override void beginParagraph(TextAlignment alignment, float spacingBefore, float spacingAfter)
        {
            fParagraph = fDocument.addParagraph();
            fParagraph.Alignment = iAlignments[(int)alignment];
            
            Margins margins = fParagraph.Margins;
            margins[Direction.Top] = spacingBefore;
            margins[Direction.Bottom] = spacingAfter;
        }

        public override void endParagraph()
        {
        }

        public override void addParagraphChunk(string text, object font)
        {
            addParagraphChunk(fParagraph, text, font);
        }

        public override void addParagraphChunkAnchor(string text, object font, string anchor)
        {
            RtfCharFormat fmt = addParagraphChunk(fParagraph, text, font);
            fmt.Bookmark = anchor;
        }

        public override void addParagraphChunkLink(string text, object font, string link, object linkFont, bool sup)
        {
            RtfCharFormat fmt = addParagraphChunk(fParagraph, text, font);
            if (sup) fmt.FontStyle.addStyle(FontStyleFlag.Super);
            fmt.LocalHyperlink = link;
        }

        public override void addNote(string text, object font)
        {
            
        }
    }
}
