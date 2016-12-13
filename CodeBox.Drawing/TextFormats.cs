﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBox.Drawing
{
    public static class TextFormats
    {
        public static readonly StringFormat Compact = new StringFormat(StringFormat.GenericTypographic)
        {
            LineAlignment = StringAlignment.Near,
            Alignment = StringAlignment.Near,
            Trimming = StringTrimming.None,
            FormatFlags = StringFormatFlags.NoWrap
        };

        public static readonly StringFormat Path = new StringFormat(StringFormat.GenericTypographic)
        {
            LineAlignment = StringAlignment.Near,
            Alignment = StringAlignment.Near,
            Trimming = StringTrimming.EllipsisPath,
            FormatFlags = StringFormatFlags.NoWrap
        };

        public static readonly StringFormat Centered = new StringFormat(StringFormat.GenericTypographic)
        {
            LineAlignment = StringAlignment.Center,
            Alignment = StringAlignment.Near,
            Trimming = StringTrimming.None,
            FormatFlags = StringFormatFlags.NoWrap
        };
    }
}