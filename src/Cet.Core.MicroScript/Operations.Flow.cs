using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cet.Core.MicroScript
{
    [Instruct(Name = "jmp", Mode = OperandMode.Label)]
    internal class OpJump : IOperation, IOperationLabel
    {
        public string LabelName { get; set; }
        public int Position { get; set; }

        void IOperation.Exec(DataStack stack, IDataAccess extdata)
        {
            stack.InstructionIndex = this.Position - 1;
        }
    }


    [Instruct(Name = "je", Mode = OperandMode.Label)]
    internal class OpJumpEqual : IOperation, IOperationLabel
    {
        public string LabelName { get; set; }
        public int Position { get; set; }

        void IOperation.Exec(DataStack stack, IDataAccess extdata)
        {
            object oa = stack.Pop();

            bool isTrue = OperationHelpers.EvaluateAsLogicalTrue(oa);
            if (isTrue == false)
            {
                stack.InstructionIndex = this.Position - 1;
            }
        }
    }


    [Instruct(Name = "jne", Mode = OperandMode.Label)]
    internal class OpJumpNotEqual : IOperation, IOperationLabel
    {
        public string LabelName { get; set; }
        public int Position { get; set; }

        void IOperation.Exec(DataStack stack, IDataAccess extdata)
        {
            object oa = stack.Pop();

            bool isTrue = OperationHelpers.EvaluateAsLogicalTrue(oa);
            if (isTrue)
            {
                stack.InstructionIndex = this.Position - 1;
            }
        }
    }
}
