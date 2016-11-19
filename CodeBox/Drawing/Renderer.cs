﻿using CodeBox.Styling;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBox.Drawing
{
    internal sealed class Renderer
    {
        private readonly Editor editor;
        private static readonly StringFormat format = new StringFormat(StringFormat.GenericTypographic)
        {
            LineAlignment = StringAlignment.Center,
            Alignment = StringAlignment.Center,
            Trimming = StringTrimming.None
        };

        public Renderer(Editor editor)
        {
            this.editor = editor;
        }

        internal int DrawLineLengthIndicator(Graphics g, int len, int x, int y)
        {
            if (x + editor.Scroll.ScrollPosition.X < editor.Info.TextLeft)
                return 0;

            var shift = editor.ShowEol ? editor.Info.CharWidth / 2 : 0;
            x += shift;
            var str = len.ToString();
            g.DrawString(str, editor.Settings.SmallFont, editor.Styles.SpecialSymbol.ForeColor.Brush(),
                new PointF(x, y), Style.Format);
            return shift + (str.Length + 1) * editor.Info.CharWidth;
        }

        internal void DrawLongLineIndicators(Graphics g)
        {
            if (editor.WordWrap)
                return;

            foreach (var i in editor.Settings.LongLineIndicators)
            {
                var x = editor.Info.TextLeft + i * editor.Info.CharWidth
                    + editor.Scroll.ScrollPosition.X;

                if (x <= editor.Info.TextLeft)
                    continue;

                g.DrawLine(editor.Styles.SpecialSymbol.ForeColor.Pen(),
                    x, editor.Info.TextTop, x, editor.Info.TextBottom);
            }
        }

        internal void DrawWordWrapColumn(Graphics g)
        {
            if (!editor.WordWrap || editor.WordWrapColumn == 0)
                return;

            var x = editor.Info.TextLeft + editor.WordWrapColumn 
                * editor.Info.CharWidth + editor.Scroll.ScrollPosition.X;

            if (x > editor.Info.TextLeft)
                g.DrawLine(editor.Styles.SpecialSymbol.ForeColor.Pen(),
                    x, editor.Info.TextTop, x, editor.Info.TextBottom);
        }

        internal int DrawFoldingIndicator(Graphics g, int x, int y)
        {
            var w = editor.Info.CharWidth * 3;
            g.FillRectangle(editor.Settings.FoldingActiveForeColor.Brush(),
                new Rectangle(x, y + editor.Info.LineHeight / 4, w, editor.Info.LineHeight / 2));
            g.DrawString("···", editor.Settings.Font.Get(editor.Styles.Default.FontStyle),
                editor.Settings.FoldingBackColor.Brush(),
                new Point(x, y), Style.Format);
            return w;
        }
    }
}
