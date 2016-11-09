using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cet.Core.MicroScript
{
    [Instruct(Name = "push", Mode = OperandMode.Imm1)]
    internal class OpPushImmediate : IOperation, IOperationImmediate1
    {
        public object Value1 { get; set; }

        void IOperation.Exec(DataStack stack, IDataAccess xdata)
        {
            stack.Push(this.Value1);
        }
    }


    [Instruct(Name = "push", Mode = OperandMode.Reg1)]
    internal class OpPushRegister : IOperation, IOperationRegister1
    {
        public string RegName1 { get; set; }

        void IOperation.Exec(DataStack stack, IDataAccess xdata)
        {
            stack.Push(
                stack.Registers[this.RegName1]
                );
        }
    }


    [Instruct(Name = "push", Mode = OperandMode.Dir1)]
    internal class OpPushDirect : IOperation, IOperationDirect1
    {
        public string VarName1 { get; set; }

        void IOperation.Exec(DataStack stack, IDataAccess xdata)
        {
            stack.Push(
                xdata.GetData(this.VarName1)
                );
        }
    }


    [Instruct(Name = "pop", Mode = OperandMode.Reg1)]
    internal class OpPopRegister : IOperation, IOperationRegister1
    {
        public string RegName1 { get; set; }

        void IOperation.Exec(DataStack stack, IDataAccess xdata)
        {
            stack.Registers[this.RegName1] = stack.Pop();
        }
    }


    [Instruct(Name = "pop", Mode = OperandMode.Dir1)]
    internal class OpPopDirect : IOperation, IOperationDirect1
    {
        public string VarName1 { get; set; }

        void IOperation.Exec(DataStack stack, IDataAccess xdata)
        {
            xdata.SetData(this.VarName1, stack.Pop());
        }
    }


    [Instruct(Name = "mov", Mode = OperandMode.Reg1 | OperandMode.Imm2)]
    internal class OpMoveRegisterImmediate : IOperation, IOperationRegister1, IOperationImmediate2
    {
        public string RegName1 { get; set; }
        public object Value2 { get; set; }

        void IOperation.Exec(DataStack stack, IDataAccess xdata)
        {
            stack.Registers[this.RegName1] = this.Value2;
        }
    }


    [Instruct(Name = "mov", Mode = OperandMode.Reg1 | OperandMode.Reg2)]
    internal class OpMoveRegisterRegister : IOperation, IOperationRegister1, IOperationRegister2
    {
        public string RegName1 { get; set; }
        public string RegName2 { get; set; }

        void IOperation.Exec(DataStack stack, IDataAccess xdata)
        {
            stack.Registers[this.RegName1] = stack.Registers[this.RegName2];
        }
    }


    [Instruct(Name = "mov", Mode = OperandMode.Reg1 | OperandMode.Dir2)]
    internal class OpMoveRegisterDirect : IOperation, IOperationRegister1, IOperationDirect2
    {
        public string RegName1 { get; set; }
        public string VarName2 { get; set; }

        void IOperation.Exec(DataStack stack, IDataAccess xdata)
        {
            stack.Registers[this.RegName1] = xdata.GetData(this.VarName2);
        }
    }


    [Instruct(Name = "mov", Mode = OperandMode.Dir1 | OperandMode.Imm2)]
    internal class OpMoveDirectImmediate : IOperation, IOperationDirect1, IOperationImmediate2
    {
        public string VarName1 { get; set; }
        public object Value2 { get; set; }

        void IOperation.Exec(DataStack stack, IDataAccess xdata)
        {
            xdata.SetData(this.VarName1, this.Value2);
        }
    }


    [Instruct(Name = "mov", Mode = OperandMode.Dir1 | OperandMode.Reg2)]
    internal class OpMoveDirectRegister : IOperation, IOperationDirect1, IOperationRegister2
    {
        public string VarName1 { get; set; }
        public string RegName2 { get; set; }

        void IOperation.Exec(DataStack stack, IDataAccess xdata)
        {
            xdata.SetData(this.VarName1, stack.Registers[this.RegName2]);
        }
    }


    [Instruct(Name = "mov", Mode = OperandMode.Dir1 | OperandMode.Dir2)]
    internal class OpMoveDirectDirect : IOperation, IOperationDirect1, IOperationDirect2
    {
        public string VarName1 { get; set; }
        public string VarName2 { get; set; }

        void IOperation.Exec(DataStack stack, IDataAccess xdata)
        {
            xdata.SetData(this.VarName1, xdata.GetData(this.VarName2));
        }
    }
}
