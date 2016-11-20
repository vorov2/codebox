﻿using System;
using CodeBox.ObjectModel;
using CodeBox.ComponentModel;
using System.ComponentModel.Composition;
using CodeBox.Core.ComponentModel;

namespace CodeBox.Commands
{
    [Export(typeof(IComponent))]
    [ComponentData("command.editor.extendright")]
    public sealed class ExtendRightCommand : SelectionCommand
    {
        protected override Pos Select(Selection sel) => RightCommand.MoveRight(View, sel);

        public override bool SupportLimitedMode => true;
    }
}
