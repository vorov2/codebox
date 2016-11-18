﻿using System;
using System.Collections.Generic;
using System.Linq;
using CodeBox.ObjectModel;
using CodeBox.ComponentModel;
using CodeBox.Affinity;
using System.ComponentModel.Composition;
using static CodeBox.Commands.ActionResults;

namespace CodeBox.Commands
{
    [Export(typeof(IComponent))]
    [ComponentData("command.editor.newline")]
    public sealed class InsertNewLineCommand : EditorCommand
    {
        private IEnumerable<Character> @string;
        private Pos undoPos;
        private Selection redoSel;
        private int indent;
        private IEnumerable<Character> unindent;

        public override ActionResults Execute(Selection selection)
        {
            undoPos = selection.Start;
            redoSel = selection.Clone();

            if (!selection.IsEmpty)
                @string = DeleteRangeCommand.DeleteRange(Context, selection);
            
            var pos = InsertNewLine(Document, undoPos);
            selection.Clear(pos);


            var indentKey = Context.AffinityManager.GetAffinity(new Pos(pos.Line, 0)).GetIndentComponentKey(Context);
            var comp = ComponentCatalog.Instance.GetComponent<IDentComponent>(indentKey);
            indent = comp != null ? comp.CalculateIndentation(Context, pos.Line) : 0;

            if (indent > 0)
            {
                if (pos.Line > 0)
                {
                    var ln = Document.Lines[pos.Line - 1];

                    if (ln.IsEmpty() && ln.Length > 0)
                    {
                        unindent = ln.ToList();
                        ln.RemoveRange(0, ln.Length);
                    }
                }

                var str = Context.UseTabs ? new string('\t', indent / Context.IndentSize)
                    : new string(' ', indent);
                Document.Lines[pos.Line].Insert(0, str.MakeCharacters());
                selection.Clear(new Pos(pos.Line, pos.Col + str.Length));
            }

            return Change;
        }

        public override ActionResults Redo(out Pos pos)
        {
            @string = null;
            var sel = redoSel;
            Execute(sel);
            pos = sel.Caret;
            return Change;
        }

        public override ActionResults Undo(out Pos pos)
        {
            pos = undoPos;

            if (@string != null)
                pos = InsertRangeCommand.InsertRange(Document, undoPos, @string);

            var line = Document.Lines[pos.Line];
            var nextLine = Document.Lines[pos.Line + 1];
            Document.Lines.Remove(nextLine);

            if (indent > 0)
            {
                var real = Context.UseTabs ? indent / Context.IndentSize : indent;
                nextLine.RemoveRange(0, real);
            }

            if (unindent != null)
                line.Insert(0, unindent);

            line.Append(nextLine);
            return Change;
        }

        internal static Pos InsertNewLine(Document doc, Pos pos)
        {
            var ln = doc.Lines[pos.Line];
            var str = default(IEnumerable<Character>);

            if (pos.Col != ln.Length && pos.Col < ln.Length)
            {
                str = ln.GetRange(pos.Col, ln.Length - pos.Col);
                ln.RemoveRange(pos.Col, ln.Length - pos.Col);
            }

            var newLn = doc.NewLine(str);
            doc.Lines.Insert(pos.Line + 1, newLn);
            pos = new Pos(pos.Line + 1, 0);
            return pos;
        }

        public override IEditorCommand Clone()
        {
            return new InsertNewLineCommand();
        }

        public override bool ModifyContent => true;
    }
}
