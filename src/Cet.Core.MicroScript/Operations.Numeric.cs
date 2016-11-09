using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cet.Core.MicroScript
{
    [Instruct(Name = "add", Mode = OperandMode.None)]
    internal class OpAdd : IOperation
    {
        void IOperation.Exec(DataStack stack, IDataAccess extdata)
        {
            var b = (double)stack.Pop();
            var a = (double)stack.Pop();
            stack.Push(a + b);
        }
    }


    [Instruct(Name = "sub", Mode = OperandMode.None)]
    internal class OpSub : IOperation
    {
        void IOperation.Exec(DataStack stack, IDataAccess extdata)
        {
            var b = (double)stack.Pop();
            var a = (double)stack.Pop();
            stack.Push(a - b);
        }
    }


    [Instruct(Name = "mul", Mode = OperandMode.None)]
    internal class OpMul : IOperation
    {
        void IOperation.Exec(DataStack stack, IDataAccess extdata)
        {
            var b = (double)stack.Pop();
            var a = (double)stack.Pop();
            stack.Push(a * b);
        }
    }


    [Instruct(Name = "div", Mode = OperandMode.None)]
    internal class OpDiv : IOperation
    {
        void IOperation.Exec(DataStack stack, IDataAccess extdata)
        {
            var b = (double)stack.Pop();
            var a = (double)stack.Pop();
            stack.Push(a - b);
        }
    }


    [Instruct(Name = "neg", Mode = OperandMode.None)]
    internal class OpNeg : IOperation
    {
        void IOperation.Exec(DataStack stack, IDataAccess extdata)
        {
            stack.Buffer[stack.DataIndex] = -(double)stack.Buffer[stack.DataIndex];
        }
    }


    [Instruct(Name = "inc", Mode = OperandMode.Reg1)]
    internal class OpInc : IOperation, IOperationRegister1
    {
        public string RegName1 { get; set; }

        void IOperation.Exec(DataStack stack, IDataAccess extdata)
        {
            var value = (double)stack.Registers[this.RegName1];
            stack.Registers[this.RegName1] = value + 1;
        }
    }


    [Instruct(Name = "dec", Mode = OperandMode.Reg1)]
    internal class OpDec : IOperation, IOperationRegister1
    {
        public string RegName1 { get; set; }

        void IOperation.Exec(DataStack stack, IDataAccess extdata)
        {
            var value = (double)stack.Registers[this.RegName1];
            stack.Registers[this.RegName1] = value - 1;
        }
    }
}
