using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cet.Core.MicroScript
{
    internal sealed class DataStack
    {
        internal DataStack() { }


        //data buffer (stack) exposed for faster access
        internal object[] Buffer = new object[100];
        internal int DataIndex = -1;

        //instruction pointer
        internal int InstructionIndex = -1;


        /// <summary>
        /// Private registers bag
        /// </summary>
        internal Dictionary<string, object> Registers { get; } = new Dictionary<string, object>();


        /// <summary>
        /// Push a data on the stack
        /// </summary>
        /// <param name="data"></param>
        internal void Push(object data)
        {
            this.Buffer[++this.DataIndex] = data;
        }


        /// <summary>
        /// Pop a data from the stack
        /// </summary>
        /// <returns></returns>
        internal object Pop()
        {
            var data = this.Buffer[this.DataIndex];
            this.Buffer[this.DataIndex--] = null;
            return data;
        }

    }
}
