using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cet.Core.MicroScript
{
    /// <summary>
    /// Basic engine class to execute a compiled script
    /// </summary>
    public sealed class ScriptEngine
    {
        /// <summary>
        /// Compiled code stream
        /// </summary>
        public CompiledStream CodeStream { get; set; }


        /// <summary>
        /// Execute the compiled script
        /// </summary>
        /// <param name="xdata"></param>
        /// <returns></returns>
        public RunResult Execute(IDataAccess xdata)
        {
            var cstream = this.CodeStream;

            var stack = new DataStack();
            while (++stack.InstructionIndex < cstream.InstructList.Count)
            {
                cstream.InstructList[stack.InstructionIndex].Exec(stack, xdata);
            }

            var result = new RunResult();
            switch (stack.DataIndex)
            {
                case -1:
                    break;

                case 0:
                    result.Result = stack.Pop();
                    result.HasResult = true;
                    break;

                default:
                    result.Error = new RuntimeException(
                        $"The data stack still contains {stack.DataIndex + 1} items."
                        );
                    break;
            }
            return result;
        }

    }


    /// <summary>
    /// Resulting info from the execution
    /// </summary>
    public sealed class RunResult
    {
        internal RunResult() { }

        public bool HasResult { get; internal set; }
        public object Result { get; internal set; }
        public Exception Error { get; internal set; }
    }


    /// <summary>
    /// Generic execution exception
    /// </summary>
    public class RuntimeException : Exception
    {
        public RuntimeException(string message) : base(message) { }
        public RuntimeException(string message, Exception innerException) : base(message, innerException) { }
    }


    /// <summary>
    /// Compiled script stream holder
    /// </summary>
    public sealed class CompiledStream
    {
        internal CompiledStream() { }

        internal List<IOperation> InstructList { get; } = new List<IOperation>();
    }
}
