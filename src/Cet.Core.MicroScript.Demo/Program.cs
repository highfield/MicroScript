using Cet.Core.MicroScript;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MicroScriptCoreDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CompilerSample0();

            Sample1_CLR();
            Sample1_CompileExecuteMany();
            Sample1_CompileOnceExecuteMany();
            Sample3_CompileOnceExecuteMany();

            Fibonacci();

            Console.WriteLine("Complete.");
            Console.ReadKey();
        }


        static void CompilerSample0()
        {
            string text = System.IO.File.ReadAllText("samples/sample0.txt");
            var cstream = Cet.Core.MicroScript.Compiler.Compile(text);
            //place a breakpoint here and check the instruction list
        }


        static void Sample1_CLR()
        {
            const int N = 100000;
            var sw = Stopwatch.StartNew();

            double total = 0;
            for (int i = 0; i < N; i++)
            {
                double a = i;
                double b = 2 * i;

                total += (a + b);
            }

            sw.Stop();
            Console.WriteLine($"Sample1_CLR: total={total}; ms={sw.ElapsedMilliseconds}; ms/cyc={sw.ElapsedMilliseconds * 1.0 / N}");
            Console.WriteLine();
        }


        static void Sample1_CompileExecuteMany()
        {
            string text = System.IO.File.ReadAllText("samples/sample1.txt");

            const int N = 100000;
            var sw = Stopwatch.StartNew();

            double total = 0;
            for (int i = 0; i < N; i++)
            {
                var engine = new Cet.Core.MicroScript.ScriptEngine();
                engine.CodeStream = Cet.Core.MicroScript.Compiler.Compile(text);

                var xdata = new MyDataAccess();
                xdata.Parameters.Add("a", 1.0 * i);
                xdata.Parameters.Add("b", 2.0 * i);

                RunResult engres = engine.Execute(xdata);
                total += (double)engres.Result;
            }

            sw.Stop();
            Console.WriteLine($"Sample1_CompileExecuteMany: total={total}; ms={sw.ElapsedMilliseconds}; ms/cyc={sw.ElapsedMilliseconds * 1.0 / N}");
            Console.WriteLine();
        }


        static void Sample1_CompileOnceExecuteMany()
        {
            string text = System.IO.File.ReadAllText("samples/sample1.txt");

            var engine = new Cet.Core.MicroScript.ScriptEngine();
            engine.CodeStream = Cet.Core.MicroScript.Compiler.Compile(text);

            const int N = 100000;
            var sw = Stopwatch.StartNew();

            double total = 0;
            for (int i = 0; i < N; i++)
            {
                var xdata = new MyDataAccess();
                xdata.Parameters.Add("a", 1.0 * i);
                xdata.Parameters.Add("b", 2.0 * i);

                RunResult engres = engine.Execute(xdata);
                total += (double)engres.Result;
            }

            sw.Stop();
            Console.WriteLine($"Sample1_CompileOnceExecuteMany: total={total}; ms={sw.ElapsedMilliseconds}; ms/cyc={sw.ElapsedMilliseconds * 1.0 / N}");
            Console.WriteLine();
        }


        static void Sample3_CompileOnceExecuteMany()
        {
            string text = System.IO.File.ReadAllText("samples/sample3.txt");

            const int N = 100000;
            var sw = Stopwatch.StartNew();

            var engine = new Cet.Core.MicroScript.ScriptEngine();
            engine.CodeStream = Cet.Core.MicroScript.Compiler.Compile(text);

            var xdata = new MyDataAccess();
            xdata.Parameters.Add("N", 1.0 * N);

            var total = (double)engine.Execute(xdata).Result;

            sw.Stop();
            Console.WriteLine($"Sample3_CompileOnceExecuteMany: total={total}; ms={sw.ElapsedMilliseconds}; ms/cyc={sw.ElapsedMilliseconds * 1.0 / N}");
            Console.WriteLine();
        }


        static void Fibonacci()
        {
            string text = System.IO.File.ReadAllText("samples/fib.txt");

            var sw = Stopwatch.StartNew();

            var engine = new Cet.Core.MicroScript.ScriptEngine();
            engine.CodeStream = Cet.Core.MicroScript.Compiler.Compile(text);

            var xdata = new MyDataAccess();
            engine.Execute(xdata);

            sw.Stop();
            Console.WriteLine($"Fibonacci: ms={sw.ElapsedMilliseconds}");
            Console.WriteLine();
        }


        class MyDataAccess : Cet.Core.MicroScript.IDataAccess
        {
            public Dictionary<string, object> Parameters = new Dictionary<string, object>();

            public object GetData(string name)
            {
                return this.Parameters[name];
            }

            public void SetData(string name, object value)
            {
                if (name == "out")
                {
                    Console.WriteLine(value);
                }
            }
        }

    }
}
