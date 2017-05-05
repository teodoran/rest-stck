using System;
using System.Collections.Generic;
using Microsoft.FSharp.Collections;

namespace RestStck.Models
{
    public class EvaluationResult
    {
        public FSharpList<Int32> Stack { get; set; }
        public FSharpList<string> Expression { get; set; }
        public string Error { get; set; }
        public Dictionary<string, HalLink> _links { get; set; }
    }
}