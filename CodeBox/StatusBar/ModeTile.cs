﻿using CodeBox.Core;
using CodeBox.Core.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBox.StatusBar
{
    public sealed class ModeTile : StatusBarTile
    {
        private readonly Editor editor;
        private string lastGrammar;

        public ModeTile(Editor editor) : base(TileAlignment.Right)
        {
            this.editor = editor;
        }

        public override string Text
        {
            get
            {
                return (lastGrammar == null && editor.Buffer.GrammarKey != null)
                        || (editor.Buffer.GrammarKey != lastGrammar && editor.Buffer.GrammarKey != null)
                    ? (lastGrammar = App.Ext.Grammars().GetGrammar(editor.Buffer.GrammarKey).Name)
                    : lastGrammar;
            }
            set { base.Text = value; }
        }

        protected internal override void PerformClick()
        {
            App.Ext.Run(editor, Cmd.ChangeBufferMode);
            base.PerformClick();
        }
    }

}
