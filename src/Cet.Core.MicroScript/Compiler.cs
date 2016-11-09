using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Cet.Core.MicroScript
{
    public static partial class Compiler
    {
        static Compiler()
        {
            //scan the assembly to search all the available instructions
            Assembly asm = typeof(Compiler).GetTypeInfo().Assembly;
            foreach (TypeInfo type in asm.DefinedTypes)
            {
                var attr = type
                    .GetCustomAttributes(typeof(InstructAttribute), true)
                    .OfType<InstructAttribute>()
                    .FirstOrDefault();

                if (attr != null)
                {
                    InstructEntryDescriptor entry;
                    if (_opmap.TryGetValue(attr.Name, out entry) == false)
                    {
                        entry = new InstructEntryDescriptor() { Name = attr.Name };
                        _opmap.Add(entry.Name, entry);
                    }

                    var ds = new InstructActualDescriptor();
                    ds.Mode = attr.Mode;
                    ds.Type = type.AsType();
                    entry.DsList.Add(ds);
                }
            }   
        }


        private static readonly Dictionary<string, InstructEntryDescriptor> _opmap = new Dictionary<string, InstructEntryDescriptor>();


        private class CompilationContext
        {
            public CompiledStream CodeStream { get; } = new CompiledStream();
            public Dictionary<string, int> LabelMap { get; } = new Dictionary<string, int>();

            public int LineNum { get; internal set; }
        }


        /// <summary>
        /// Entry point for the compilation of a text script
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static CompiledStream Compile(string text)
        {
            //create a compilation context
            var cctx = new CompilationContext();
            cctx.LineNum = 1;

            //scan for lines and parse them
            foreach (string line in LineScanner(cctx, text))
            {
                ParseLine(cctx, line);
            }

            //translate label name to position
            foreach (var ilabel in cctx.CodeStream.InstructList.OfType<IOperationLabel>())
            {
                int position;
                if (cctx.LabelMap.TryGetValue(ilabel.LabelName, out position))
                {
                    ilabel.Position = position;
                }
                else
                {
                    throw new CompilationException(
                        $"Undefined label reference: {ilabel.LabelName}"
                        );
                }
            }

            return cctx.CodeStream;
        }


        /// <summary>
        /// Simple line scanner
        /// </summary>
        /// <param name="cctx"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private static IEnumerable<string> LineScanner(
            CompilationContext cctx,
            IEnumerable<char> buffer
            )
        {
            bool flag = false;
            var output = new char[1024];
            int outlen = 0;

            foreach (char c in buffer)
            {
                if (c == 13 || c == 10)
                {
                    //mark the end of line
                    flag = true;
                }
                else
                {
                    if (flag)
                    {
                        //skip empty lines
                        if (outlen != 0)
                        {
                            yield return new string(output, 0, outlen);
                        }

                        cctx.LineNum++;
                        flag = false;
                        outlen = 0;
                    }

                    //collect the current char in the temporary buffer
                    output[outlen++] = c;
                }
            }

            if (outlen != 0)
            {
                //yield the very last line
                yield return new string(output, 0, outlen);
                cctx.LineNum++;
            }
        }


        private class ParserResult<T>
        {
            public T Data;
            public int ConsumedCount;
            public Exception Error;
        }


        private class ParserContext
        {
            public char[] Buffer;
            public int Index;

            public ParserContext CreateChild()
            {
                var instance = new ParserContext();
                instance.Buffer = this.Buffer;
                instance.Index = this.Index;
                return instance;
            }
        }

    }


    /// <summary>
    /// Generic compilation exception
    /// </summary>
    public class CompilationException : Exception
    {
        public CompilationException(string message) : base(message) { }
        public CompilationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
