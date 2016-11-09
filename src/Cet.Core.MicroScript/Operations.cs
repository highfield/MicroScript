using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Cet.Core.MicroScript
{
    /// <summary>
    /// Interface for any executable operation
    /// </summary>
    internal interface IOperation
    {
        void Exec(DataStack stack, IDataAccess extdata);
    }


    /// <summary>
    /// Instruction having an immediate value as the first operand
    /// </summary>
    internal interface IOperationImmediate1
    {
        object Value1 { get; set; }
    }


    /// <summary>
    /// Instruction having an immediate value as the second operand
    /// </summary>
    internal interface IOperationImmediate2
    {
        object Value2 { get; set; }
    }


    /// <summary>
    /// Instruction having a register reference as the first operand
    /// </summary>
    internal interface IOperationRegister1
    {
        string RegName1 { get; set; }
    }


    /// <summary>
    /// Instruction having a register reference as the second operand
    /// </summary>
    internal interface IOperationRegister2
    {
        string RegName2 { get; set; }
    }


    /// <summary>
    /// Instruction having an external data reference as the first operand
    /// </summary>
    internal interface IOperationDirect1
    {
        string VarName1 { get; set; }
    }


    /// <summary>
    /// Instruction having an external data reference as the second operand
    /// </summary>
    internal interface IOperationDirect2
    {
        string VarName2 { get; set; }
    }


    /// <summary>
    /// Instruction having a label reference as operand
    /// </summary>
    internal interface IOperationLabel
    {
        string LabelName { get; set; }
        int Position { get; set; }
    }


    [Flags]
    internal enum OperandMode
    {
        None = 0,
        Label = 1,

        Imm1 = 0x10,
        Reg1 = 0x20,
        Dir1 = 0x40,

        Imm2 = 0x0100,
        Reg2 = 0x0200,
        Dir2 = 0x0400,
    }


    internal class InstructAttribute : Attribute
    {
        public string Name;
        public OperandMode Mode;
    }


    internal class InstructEntryDescriptor
    {
        public string Name;
        public List<InstructActualDescriptor> DsList { get; } = new List<InstructActualDescriptor>();
    }


    internal class InstructActualDescriptor
    {
        public OperandMode Mode;
        public Type Type;
    }
}
