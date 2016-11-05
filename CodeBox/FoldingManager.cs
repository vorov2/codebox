﻿using CodeBox.Folding;
using CodeBox.ObjectModel;
using CodeBox.Styling;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBox
{
    public sealed class FoldingManager
    {
        private readonly Editor editor;

        internal FoldingManager(Editor editor)
        {
            this.editor = editor;
        }

        public void ExpandAll()
        {
            foreach (var ln in editor.Lines)
                ln.Folding &= ~FoldingStates.Invisible;
        }

        public void CollapseAll()
        {
            foreach (var ln in editor.Lines)
                if (!ln.Folding.Has(FoldingStates.Header))
                    ln.Folding |= FoldingStates.Invisible;
        }

        public bool IsCollapsedHeader(int lineIndex)
        {
            if (lineIndex + 1 >= editor.Lines.Count)
                return false;

            var ln = editor.Lines[lineIndex];
            return ln.Folding.Has(FoldingStates.Header) && lineIndex < editor.Lines.Count
                && editor.Lines[lineIndex + 1].Folding.Has(FoldingStates.Invisible);
        }

        public void ToggleExpand(int lineIndex)
        {
            var ln = editor.Lines[lineIndex];
            var vis = lineIndex < editor.Lines.Count
                ? editor.Lines[lineIndex + 1].Folding.Has(FoldingStates.Invisible) : true;

            if (ln.Folding.Has(FoldingStates.Header))
            {
                var foldLevel = ln.FoldingLevel;
                var selPos = new Pos(lineIndex, ln.Length);

                for (var i = lineIndex + 1; i < editor.Lines.Count; i++)
                {
                    var cln = editor.Lines[i];

                    if (cln.FoldingLevel <= foldLevel && cln.FoldingLevel != 0)
                        break;

                    if (vis)
                    {
                        cln.Folding &= ~FoldingStates.Invisible;
                    }
                    else
                    {
                        cln.Folding |= FoldingStates.Invisible;

                        foreach (var s in editor.Buffer.Selections)
                        {
                            var start = s.Start > s.End ? s.End : s.Start;
                            var end = s.Start > s.End ? s.Start : s.End;

                            if (i >= start.Line && i <= s.End.Line)
                                s.Clear(selPos);
                        }
                    }
                }
            }
        }

        internal void RebuildFolding()
        {
            if (editor.Lines.Count == 0)
                return;

            Task.Run(() =>
            {
                var fvl = editor.Scroll.FirstVisibleLine;
                var lvl = editor.Scroll.LastVisibleLine;

                while (fvl > -1 && !IsFoldingHeader(fvl))
                    fvl--;

                fvl = fvl < 0 ? 0 : fvl;
                lvl = lvl < fvl ? fvl : lvl;

                var range = new Range(
                        new Pos(fvl, 0),
                        new Pos(lvl, editor.Lines[lvl].Length - 1));
                var fp = Provider;

                if (fp == null)
                    OnFoldingNeeded(range);
                else
                    fp.Fold(editor.Context, range);
            });
        }

        internal void DrawFoldingIndicator(Graphics g, int x, int y)
        {
            g.FillRectangle(editor.CachedBrush.Create(editor.Settings.FoldingActiveForeColor),
                new Rectangle(x, y + editor.Info.LineHeight / 4, editor.Info.CharWidth * 3, editor.Info.LineHeight / 2));
            g.DrawString("···", editor.Styles.Default.Font,
                editor.CachedBrush.Create(editor.Settings.FoldingBackColor),
                new Point(x, y), TextStyle.Format);
        }

        public void SetFoldingLevel(int line, int level)
        {
            var ln = editor.Lines[line];
            ln.Folding &= ~FoldingStates.Header;
            ln.FoldingLevel = (byte)(level + 1);
        }

        public void SetFoldingHeader(int line)
        {
            var ln = editor.Lines[line];
            ln.Folding = FoldingStates.Header
                | (ln.Folding.Has(FoldingStates.Invisible) ? FoldingStates.Invisible : FoldingStates.None);
        }

        public bool IsFoldingHeader(int line) => editor.Lines[line].Folding.Has(FoldingStates.Header);

        public event EventHandler<FoldingNeededEventArgs> FoldingNeeded;
        private void OnFoldingNeeded(Range range) => FoldingNeeded?.Invoke(this, new FoldingNeededEventArgs(range));

        public IFoldingProvider Provider { get; set; }
    }
}