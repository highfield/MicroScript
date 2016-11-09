using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cet.Core.MicroScript
{
    [Instruct(Name = "cmp", Mode = OperandMode.Imm1 | OperandMode.Reg2)]
    internal class OpCompareImmediateRegister : IOperation, IOperationImmediate1, IOperationRegister2
    {
        public object Value1 { get; set; }
        public string RegName2 { get; set; }

        void IOperation.Exec(DataStack stack, IDataAccess xdata)
        {
            var a = (double)this.Value1;
            var b = (double)stack.Registers[this.RegName2];
            stack.Push(a - b);
        }
    }


    [Instruct(Name = "cmp", Mode = OperandMode.Imm1 | OperandMode.Dir2)]
    internal class OpCompareImmediateDirect : IOperation, IOperationImmediate1, IOperationDirect2
    {
        public object Value1 { get; set; }
        public string VarName2 { get; set; }

        void IOperation.Exec(DataStack stack, IDataAccess xdata)
        {
            var a = (double)this.Value1;
            var b = (double)xdata.GetData(this.VarName2);
            stack.Push(a - b);
        }
    }


    [Instruct(Name = "cmp", Mode = OperandMode.Reg1 | OperandMode.Imm2)]
    internal class OpCompareRegisterImmediate : IOperation, IOperationRegister1, IOperationImmediate2
    {
        public string RegName1 { get; set; }
        public object Value2 { get; set; }

        void IOperation.Exec(DataStack stack, IDataAccess xdata)
        {
            var a = (double)stack.Registers[this.RegName1];
            var b = (double)this.Value2;
            stack.Push(a - b);
        }
    }


    [Instruct(Name = "cmp", Mode = OperandMode.Reg1 | OperandMode.Reg2)]
    internal class OpCompareRegisterRegister : IOperation, IOperationRegister1, IOperationRegister2
    {
        public string RegName1 { get; set; }
        public string RegName2 { get; set; }

        void IOperation.Exec(DataStack stack, IDataAccess xdata)
        {
            var a = (double)stack.Registers[this.RegName1];
            var b = (double)stack.Registers[this.RegName2];
            stack.Push(a - b);
        }
    }


    [Instruct(Name = "cmp", Mode = OperandMode.Reg1 | OperandMode.Dir2)]
    internal class OpCompareRegisterDirect : IOperation, IOperationRegister1, IOperationDirect2
    {
        public string RegName1 { get; set; }
        public string VarName2 { get; set; }

        void IOperation.Exec(DataStack stack, IDataAccess xdata)
        {
            var a = (double)stack.Registers[this.RegName1];
            var b = (double)xdata.GetData(this.VarName2);
            stack.Push(a - b);
        }
    }


    [Instruct(Name = "cmp", Mode = OperandMode.Dir1 | OperandMode.Imm2)]
    internal class OpCompareDirectImmediate : IOperation, IOperationDirect1, IOperationImmediate2
    {
        public string VarName1 { get; set; }
        public object Value2 { get; set; }

        void IOperation.Exec(DataStack stack, IDataAccess xdata)
        {
            var a = (double)xdata.GetData(this.VarName1);
            var b = (double)this.Value2;
            stack.Push(a - b);
        }
    }


    [Instruct(Name = "cmp", Mode = OperandMode.Dir1 | OperandMode.Reg2)]
    internal class OpCompareDirectRegister : IOperation, IOperationDirect1, IOperationRegister2
    {
        public string VarName1 { get; set; }
        public string RegName2 { get; set; }

        void IOperation.Exec(DataStack stack, IDataAccess xdata)
        {
            var a = (double)xdata.GetData(this.VarName1);
            var b = (double)stack.Registers[this.RegName2];
            stack.Push(a - b);
        }
    }


    [Instruct(Name = "cmp", Mode = OperandMode.Dir1 | OperandMode.Dir2)]
    internal class OpCompareDirectDirect : IOperation, IOperationDirect1, IOperationDirect2
    {
        public string VarName1 { get; set; }
        public string VarName2 { get; set; }

        void IOperation.Exec(DataStack stack, IDataAccess xdata)
        {
            var a = (double)xdata.GetData(this.VarName1);
            var b = (double)xdata.GetData(this.VarName2);
            stack.Push(a - b);
        }
    }
}
