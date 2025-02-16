using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Responses;

public class Response
{
    public object? data { get; set; }
    public string message { get; set; }
    public int code { get; set; }
}
