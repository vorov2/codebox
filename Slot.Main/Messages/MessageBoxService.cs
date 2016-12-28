﻿using System.ComponentModel.Composition;
using System.Windows.Forms;
using Slot.Core;
using Slot.Core.ComponentModel;
using Slot.Core.Messages;
using Slot.Core.ViewModel;

namespace Slot.Main.Messages
{
    [Export(typeof(IMessageBox))]
    [ComponentData(Name)]
    public sealed class MessageBoxService : IMessageBox
    {
        public const string Name = "messages.default";

        public MessageButtons Show(string caption, string text, MessageButtons buttons)
        {
            var frm = new MessageWindow
            {
                Caption = caption ?? "",
                Detail = text ?? "",
                Buttons = buttons
            };
            frm.ShowDialog(Form.ActiveForm);
            var vm = App.Catalog<IViewManager>().Default().GetActiveView();
            ((Control)vm.Editor).Focus();
            return frm.ButtonClicked;
        }
    }
}