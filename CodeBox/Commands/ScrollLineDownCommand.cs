﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBox.ObjectModel;
using static CodeBox.Commands.ActionExponent;

namespace CodeBox.Commands
{
    [CommandBehavior(None)]
    public sealed class ScrollLineDownCommand : Command
    {
        public override ActionResults Execute(CommandArgument arg, Selection sel)
        {
            Context.Scroll.ScrollY(-1);
            return ActionResults.Clean | ActionResults.AutocompleteKeep;
        }
    }
}
