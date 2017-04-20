using System;
using System.Text;
using System.Security.Cryptography;
using Microsoft.FSharp.Collections;

namespace RestStck.Models
{
    public class EvaluationContext
    {
        public FSharpList<Tuple<string,FSharpList<String>>> Heap { get; set; }
        public FSharpList<Int32> Stack { get; set; }
        
        public string Key { get; private set; }

        public string Hash()
        {
            var hash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(Heap.ToString() + Stack.ToString()));
            Key = BitConverter.ToString(hash).Replace("-", "");
            return Key;
        }
    }
}