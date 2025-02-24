using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Image.Generate
{
    public enum AspectRatio
    {
        [Description("1:1")]
        Default,
        [Description("3:2")]
        Type_2,
        [Description("4:3")]
        Type_3,
        [Description("3:4")]
        Type_4,
        [Description("16:9")]
        Type_5,
        [Description("9:16")]
        Type_6,
    }
}
